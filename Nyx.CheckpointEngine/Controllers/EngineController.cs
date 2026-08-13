using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Nyx.CheckpointEngine.Persistence;
using Nyx.CheckpointEngine.Models;

namespace Nyx.CheckpointEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EngineController : ControllerBase
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EngineController(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpPost("evaluate-transition")]
    public async Task<IActionResult> EvaluateTransition([FromBody] TransitionRequest request, CancellationToken ct)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT ec.id_entity_checkpoint AS Id,
                   cc.name AS CheckpointName,
                   cc.owner_department AS OwnerDepartment
            FROM checkpoint_service.entity_checkpoint ec
            JOIN checkpoint_service.checkpoint_catalog cc ON ec.id_checkpoint_catalog = cc.id_checkpoint_catalog
            WHERE ec.entity_type = @EntityType
              AND ec.entity_id = @EntityId
              AND cc.blocks_progress = true
              AND ec.status NOT IN ('APROBADO', 'SUBSANADO');";

        var blockers = (await connection.QueryAsync<dynamic>(
            new CommandDefinition(sql, new 
            { 
                EntityType = request.EntityType, 
                EntityId = request.EntityId 
            }, cancellationToken: ct)
        )).ToList();

        if (blockers.Any())
        {
            var blockerDetails = blockers.Select(b => new BlockerDetail
            {
                Name = b.checkpointname,
                Department = b.ownerdepartment
            }).ToList();

            return Ok(new TransitionResult
            {
                CanTransition = false,
                Blockers = blockerDetails
            });
        }

        return Ok(new TransitionResult
        {
            CanTransition = true,
            Blockers = new List<BlockerDetail>()
        });
    }

    [HttpPost("trigger-checkpoints")]
    public async Task<IActionResult> TriggerCheckpoints([FromBody] TriggerRequest request, CancellationToken ct)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        // Extract campaign name from metadata
        string campaignName = string.Empty;
        if (request.Metadata != null && request.Metadata.TryGetValue("campaign", out var campaignObj) && campaignObj != null)
        {
            campaignName = campaignObj.ToString() ?? string.Empty;
        }

        // Query catalog templates triggered by this stage and not chained to KO
        const string queryCatalogSql = @"
            SELECT * 
            FROM checkpoint_service.checkpoint_catalog 
            WHERE trigger_stage_id = @TriggerStageId 
              AND trigger_on_ko_catalog_id IS NULL
              AND is_active = true;";
        
        var catalog = (await connection.QueryAsync<CheckpointCatalog>(
            new CommandDefinition(queryCatalogSql, new { TriggerStageId = request.ToStatusId }, cancellationToken: ct)
        )).ToList();

        int createdCount = 0;
        foreach (var rule in catalog)
        {
            // Verify campaign applicability
            bool applies = rule.AppliesTo == "Toda campaña nueva" || 
                           string.Equals(rule.AppliesTo, campaignName, StringComparison.OrdinalIgnoreCase);
            
            if (!applies) continue;

            // Check if already instantiated
            const string checkExistSql = @"
                SELECT COUNT(*) 
                FROM checkpoint_service.entity_checkpoint 
                WHERE entity_type = @EntityType AND entity_id = @EntityId AND id_checkpoint_catalog = @CatalogId;";
            
            var exists = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(checkExistSql, new 
                { 
                    EntityType = request.EntityType, 
                    EntityId = request.EntityId, 
                    CatalogId = rule.Id 
                }, cancellationToken: ct)
            ) > 0;

            if (!exists)
            {
                // Insert Checkpoint Instance
                const string insertSql = @"
                    INSERT INTO checkpoint_service.entity_checkpoint (entity_type, entity_id, id_checkpoint_catalog, status, created_at, updated_at)
                    VALUES (@EntityType, @EntityId, @CatalogId, 'PENDIENTE', NOW(), NOW())
                    RETURNING id_entity_checkpoint;";
                
                var entityCheckpointId = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(insertSql, new 
                    { 
                        EntityType = request.EntityType, 
                        EntityId = request.EntityId, 
                        CatalogId = rule.Id 
                    }, cancellationToken: ct)
                );

                // Populate steps
                const string queryStepsSql = @"
                    SELECT step_index, description 
                    FROM checkpoint_service.checkpoint_step 
                    WHERE id_checkpoint_catalog = @CatalogId;";
                
                var steps = (await connection.QueryAsync<dynamic>(
                    new CommandDefinition(queryStepsSql, new { CatalogId = rule.Id }, cancellationToken: ct)
                )).ToList();

                foreach (var step in steps)
                {
                    const string insertStepSql = @"
                        INSERT INTO checkpoint_service.entity_checkpoint_step_status (id_entity_checkpoint, step_index, is_completed, updated_at)
                        VALUES (@EntityCheckpointId, @StepIndex, false, NOW());";
                    
                    await connection.ExecuteAsync(
                        new CommandDefinition(insertStepSql, new { EntityCheckpointId = entityCheckpointId, StepIndex = step.step_index }, cancellationToken: ct)
                    );
                }

                createdCount++;
            }
        }

        return Ok(new { success = true, triggeredCount = createdCount });
    }

    [HttpPost("resolve-checkpoint")]
    public async Task<IActionResult> ResolveCheckpoint([FromBody] ResolveRequest request, CancellationToken ct)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open) connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Get the checkpoint to find the catalog ID and properties
            const string getCpSql = @"
                SELECT ec.*, cc.rollback_stage_id AS RollbackStageId, cc.name AS CheckpointName, cc.id_checkpoint_catalog AS CatalogId
                FROM checkpoint_service.entity_checkpoint ec
                JOIN checkpoint_service.checkpoint_catalog cc ON ec.id_checkpoint_catalog = cc.id_checkpoint_catalog
                WHERE ec.id_entity_checkpoint = @CheckpointId;";

            var cp = await connection.QueryFirstOrDefaultAsync<dynamic>(
                new CommandDefinition(getCpSql, new { CheckpointId = request.CheckpointId }, transaction: transaction, cancellationToken: ct)
            );

            if (cp == null)
            {
                return NotFound(new { message = $"Checkpoint con ID {request.CheckpointId} no encontrado." });
            }

            string entityType = cp.entity_type;
            string entityId = cp.entity_id;
            int catalogId = cp.catalogid;
            string cpName = cp.checkpointname;
            int? rollbackStageId = cp.rollbackstageid;

            // 2. Update checkpoint status
            const string updateCpSql = @"
                UPDATE checkpoint_service.entity_checkpoint
                SET status = @Status,
                    comments = @Comments,
                    checked_by = @CheckedBy,
                    checked_at = NOW(),
                    updated_at = NOW()
                WHERE id_entity_checkpoint = @CheckpointId;";

            await connection.ExecuteAsync(
                new CommandDefinition(updateCpSql, new 
                { 
                    CheckpointId = request.CheckpointId, 
                    Status = request.Status, 
                    Comments = request.Comments, 
                    CheckedBy = request.CheckedBy 
                }, transaction: transaction, cancellationToken: ct)
            );

            string infoMessage = $"Checkpoint {cpName} actualizado a {request.Status}.";
            bool rollbackRequired = false;

            // 3. Rollback & Chained Triggers logic if status is KO/RECHAZADO
            bool isKo = string.Equals(request.Status, "KO", StringComparison.OrdinalIgnoreCase) || 
                        string.Equals(request.Status, "RECHAZADO", StringComparison.OrdinalIgnoreCase);

            if (isKo)
            {
                if (rollbackStageId.HasValue)
                {
                    rollbackRequired = true;
                    infoMessage += $" Se indica rollback de la entidad a la etapa {rollbackStageId.Value}.";
                }

                // Chained triggers: trigger checkpoints that depend on this one going KO
                const string queryChainedSql = @"
                    SELECT * 
                    FROM checkpoint_service.checkpoint_catalog 
                    WHERE trigger_on_ko_catalog_id = @CatalogId
                      AND is_active = true;";

                var chainedList = (await connection.QueryAsync<CheckpointCatalog>(
                    new CommandDefinition(queryChainedSql, new { CatalogId = catalogId }, transaction: transaction, cancellationToken: ct)
                )).ToList();

                foreach (var chainedDef in chainedList)
                {
                    // Check if already instantiated
                    const string checkExistSql = @"
                        SELECT COUNT(*) 
                        FROM checkpoint_service.entity_checkpoint 
                        WHERE entity_type = @EntityType AND entity_id = @EntityId AND id_checkpoint_catalog = @CatalogId;";
                    
                    var exists = await connection.ExecuteScalarAsync<int>(
                        new CommandDefinition(checkExistSql, new 
                        { 
                            EntityType = entityType, 
                            EntityId = entityId, 
                            CatalogId = chainedDef.Id 
                        }, transaction: transaction, cancellationToken: ct)
                    ) > 0;

                    if (!exists)
                    {
                        const string insertChainedSql = @"
                            INSERT INTO checkpoint_service.entity_checkpoint (entity_type, entity_id, id_checkpoint_catalog, status, created_at, updated_at)
                            VALUES (@EntityType, @EntityId, @CatalogId, 'PENDIENTE', NOW(), NOW())
                            RETURNING id_entity_checkpoint;";
                        
                        var newCpId = await connection.ExecuteScalarAsync<int>(
                            new CommandDefinition(insertChainedSql, new 
                            { 
                                EntityType = entityType, 
                                EntityId = entityId, 
                                CatalogId = chainedDef.Id 
                            }, transaction: transaction, cancellationToken: ct)
                        );

                        // Populate steps
                        const string queryStepsSql = @"
                            SELECT step_index, description 
                            FROM checkpoint_service.checkpoint_step 
                            WHERE id_checkpoint_catalog = @CatalogId;";
                        
                        var steps = (await connection.QueryAsync<dynamic>(
                            new CommandDefinition(queryStepsSql, new { CatalogId = chainedDef.Id }, transaction: transaction, cancellationToken: ct)
                        )).ToList();

                        foreach (var step in steps)
                        {
                            const string insertStepSql = @"
                                INSERT INTO checkpoint_service.entity_checkpoint_step_status (id_entity_checkpoint, step_index, is_completed, updated_at)
                                VALUES (@EntityCheckpointId, @StepIndex, false, NOW());";
                            
                            await connection.ExecuteAsync(
                                new CommandDefinition(insertStepSql, new { EntityCheckpointId = newCpId, StepIndex = step.step_index }, transaction: transaction, cancellationToken: ct)
                            );
                        }

                        infoMessage += $" Se instanció el checkpoint encadenado '{chainedDef.Name}'.";
                    }
                }
            }

            transaction.Commit();
            return Ok(new ResolveResult
            {
                Success = true,
                RollbackRequired = rollbackRequired,
                RollbackStageId = rollbackStageId,
                EntityType = entityType,
                EntityId = entityId,
                Message = infoMessage
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return StatusCode(500, new { message = "Error al resolver checkpoint en el motor.", details = ex.Message });
        }
    }
}

public class TransitionRequest
{
    public string EntityType { get; set; } = "Order";
    public string EntityId { get; set; } = string.Empty;
    public int ToStatusId { get; set; }
}

public class TriggerRequest
{
    public string EntityType { get; set; } = "Order";
    public string EntityId { get; set; } = string.Empty;
    public int ToStatusId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class TransitionResult
{
    public bool CanTransition { get; set; }
    public List<BlockerDetail> Blockers { get; set; } = new();
}

public class BlockerDetail
{
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public class ResolveRequest
{
    public int CheckpointId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public int? CheckedBy { get; set; }
}

public class ResolveResult
{
    public bool Success { get; set; }
    public bool RollbackRequired { get; set; }
    public int? RollbackStageId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
