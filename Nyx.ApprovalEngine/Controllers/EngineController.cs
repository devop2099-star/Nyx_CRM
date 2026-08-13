using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Nyx.ApprovalEngine.Persistence;
using Nyx.ApprovalEngine.Models;

namespace Nyx.ApprovalEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EngineController : ControllerBase
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EngineController(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpPost("create-request")]
    public async Task<IActionResult> CreateRequest([FromBody] CreateApprovalRequestDto request, CancellationToken ct)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open) connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Extract campaign metadata
            string campaignName = string.Empty;
            if (request.Metadata != null && request.Metadata.TryGetValue("campaign", out var campaignObj) && campaignObj != null)
            {
                campaignName = campaignObj.ToString() ?? string.Empty;
            }

            // 2. Query catalog rules for this trigger stage
            const string queryCatalogSql = @"
                SELECT * 
                FROM approval_service.approval_rule_catalog 
                WHERE trigger_stage_id = @TriggerStageId 
                  AND is_active = true;";

            var rules = (await connection.QueryAsync<ApprovalRuleCatalog>(
                new CommandDefinition(queryCatalogSql, new { TriggerStageId = request.TriggerStageId }, transaction: transaction, cancellationToken: ct)
            )).ToList();

            if (!rules.Any())
            {
                return Ok(new { success = true, message = "No hay reglas de aprobación asociadas a esta etapa.", created = false });
            }

            // 3. Check if request already exists for this entity & stage
            const string checkExistSql = @"
                SELECT id_approval_request 
                FROM approval_service.entity_approval_request 
                WHERE entity_type = @EntityType 
                  AND entity_id = @EntityId 
                  AND trigger_stage_id = @TriggerStageId;";

            var existingRequestId = await connection.ExecuteScalarAsync<int?>(
                new CommandDefinition(checkExistSql, new 
                { 
                    EntityType = request.EntityType, 
                    EntityId = request.EntityId, 
                    TriggerStageId = request.TriggerStageId 
                }, transaction: transaction, cancellationToken: ct)
            );

            int approvalRequestId;
            if (existingRequestId.HasValue)
            {
                approvalRequestId = existingRequestId.Value;
            }
            else
            {
                // Create Request Header
                const string insertReqSql = @"
                    INSERT INTO approval_service.entity_approval_request 
                    (entity_type, entity_id, trigger_stage_id, overall_status, requested_by, reason, created_at, updated_at)
                    VALUES (@EntityType, @EntityId, @TriggerStageId, 'PENDING', @RequestedBy, @Reason, NOW(), NOW())
                    RETURNING id_approval_request;";

                approvalRequestId = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(insertReqSql, new 
                    { 
                        EntityType = request.EntityType, 
                        EntityId = request.EntityId, 
                        TriggerStageId = request.TriggerStageId,
                        RequestedBy = request.RequestedBy,
                        Reason = request.Reason
                    }, transaction: transaction, cancellationToken: ct)
                );
            }

            int itemsCreated = 0;
            foreach (var rule in rules)
            {
                bool applies = rule.AppliesTo == "Toda campaña nueva" || 
                               string.Equals(rule.AppliesTo, campaignName, StringComparison.OrdinalIgnoreCase);
                if (!applies) continue;

                // Check if item already exists
                const string checkItemSql = @"
                    SELECT COUNT(*) 
                    FROM approval_service.entity_approval_item 
                    WHERE id_approval_request = @RequestId AND department = @Department;";

                var itemExists = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(checkItemSql, new 
                    { 
                        RequestId = approvalRequestId, 
                        Department = rule.RequiredDepartment 
                    }, transaction: transaction, cancellationToken: ct)
                ) > 0;

                if (!itemExists)
                {
                    const string insertItemSql = @"
                        INSERT INTO approval_service.entity_approval_item 
                        (id_approval_request, id_approval_catalog, department, is_mandatory, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, status, created_at, updated_at)
                        VALUES (@RequestId, @CatalogId, @Department, @IsMandatory, @BlocksProgress, @BlocksCommission, @BlocksActivation, @BlocksLiquidation, 'PENDING', NOW(), NOW());";

                    await connection.ExecuteAsync(
                        new CommandDefinition(insertItemSql, new 
                        { 
                            RequestId = approvalRequestId, 
                            CatalogId = rule.Id, 
                            Department = rule.RequiredDepartment,
                            IsMandatory = rule.IsMandatory,
                            BlocksProgress = rule.BlocksProgress,
                            BlocksCommission = rule.BlocksCommission,
                            BlocksActivation = rule.BlocksActivation,
                            BlocksLiquidation = rule.BlocksLiquidation
                        }, transaction: transaction, cancellationToken: ct)
                    );

                    itemsCreated++;
                }
            }

            transaction.Commit();
            return Ok(new 
            { 
                success = true, 
                approvalRequestId = approvalRequestId, 
                itemsCreated = itemsCreated, 
                message = $"Solicitud de aprobación #{approvalRequestId} registrada con {itemsCreated} departamento(s) requeridos." 
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return StatusCode(500, new { message = "Error al crear solicitud de aprobación.", details = ex.Message });
        }
    }

    [HttpPost("evaluate-approval-transition")]
    public async Task<IActionResult> EvaluateApprovalTransition([FromBody] EvaluateTransitionDto request, CancellationToken ct)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT ai.id_approval_item AS Id,
                   ai.department AS Department,
                   ar.reason AS Reason
            FROM approval_service.entity_approval_item ai
            JOIN approval_service.entity_approval_request ar ON ai.id_approval_request = ar.id_approval_request
            WHERE ar.entity_type = @EntityType
              AND ar.entity_id = @EntityId
              AND ai.blocks_progress = true
              AND ai.status NOT IN ('APPROVED');";

        var blockers = (await connection.QueryAsync<dynamic>(
            new CommandDefinition(sql, new 
            { 
                EntityType = request.EntityType, 
                EntityId = request.EntityId 
            }, cancellationToken: ct)
        )).ToList();

        if (blockers.Any())
        {
            var blockerDetails = blockers.Select(b => new BlockerDetailDto
            {
                Name = $"Aprobación requerida: {b.department}",
                Department = b.department
            }).ToList();

            return Ok(new TransitionResultDto
            {
                CanTransition = false,
                Blockers = blockerDetails
            });
        }

        return Ok(new TransitionResultDto
        {
            CanTransition = true,
            Blockers = new List<BlockerDetailDto>()
        });
    }

    [HttpPost("resolve-item")]
    public async Task<IActionResult> ResolveItem([FromBody] ResolveItemDto request, CancellationToken ct)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open) connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Get the item to find parent approval request
            const string getItemSql = @"
                SELECT * 
                FROM approval_service.entity_approval_item 
                WHERE id_approval_item = @ItemId;";

            var item = await connection.QueryFirstOrDefaultAsync<EntityApprovalItem>(
                new CommandDefinition(getItemSql, new { ItemId = request.ApprovalItemId }, transaction: transaction, cancellationToken: ct)
            );

            if (item == null)
            {
                return NotFound(new { message = $"Ítem de aprobación con ID {request.ApprovalItemId} no encontrado." });
            }

            // 2. Update Item Status
            const string updateItemSql = @"
                UPDATE approval_service.entity_approval_item
                SET status = @Status,
                    comments = @Comments,
                    resolved_by = @ResolvedBy,
                    resolved_at = NOW(),
                    updated_at = NOW()
                WHERE id_approval_item = @ItemId;";

            await connection.ExecuteAsync(
                new CommandDefinition(updateItemSql, new 
                { 
                    ItemId = request.ApprovalItemId, 
                    Status = request.Status, 
                    Comments = request.Comments, 
                    ResolvedBy = request.ResolvedBy 
                }, transaction: transaction, cancellationToken: ct)
            );

            // 3. Fetch all items for this parent request to recalculate overall_status
            const string querySiblingItemsSql = @"
                SELECT * 
                FROM approval_service.entity_approval_item 
                WHERE id_approval_request = @RequestId;";

            var siblingItems = (await connection.QueryAsync<EntityApprovalItem>(
                new CommandDefinition(querySiblingItemsSql, new { RequestId = item.ApprovalRequestId }, transaction: transaction, cancellationToken: ct)
            )).ToList();

            string newOverallStatus = "PENDING";
            if (siblingItems.Any(i => i.IsMandatory && string.Equals(i.Status, "REJECTED", StringComparison.OrdinalIgnoreCase)))
            {
                newOverallStatus = "REJECTED";
            }
            else if (siblingItems.Where(i => i.IsMandatory).All(i => string.Equals(i.Status, "APPROVED", StringComparison.OrdinalIgnoreCase)))
            {
                newOverallStatus = "APPROVED";
            }

            // 4. Update Parent Request Status
            const string updateReqSql = @"
                UPDATE approval_service.entity_approval_request
                SET overall_status = @OverallStatus,
                    updated_at = NOW()
                WHERE id_approval_request = @RequestId;";

            await connection.ExecuteAsync(
                new CommandDefinition(updateReqSql, new 
                { 
                    RequestId = item.ApprovalRequestId, 
                    OverallStatus = newOverallStatus 
                }, transaction: transaction, cancellationToken: ct)
            );

            transaction.Commit();

            return Ok(new 
            { 
                success = true, 
                itemId = request.ApprovalItemId,
                itemStatus = request.Status, 
                overallStatus = newOverallStatus,
                message = $"Aprobación de {item.Department} registrada como {request.Status}. Estado global: {newOverallStatus}." 
            });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return StatusCode(500, new { message = "Error al resolver ítem de aprobación.", details = ex.Message });
        }
    }

    [HttpGet("requests/{entityType}/{entityId}")]
    public async Task<IActionResult> GetRequests(string entityType, string entityId, CancellationToken ct)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        const string queryReqSql = @"
            SELECT id_approval_request AS Id,
                   entity_type AS EntityType,
                   entity_id AS EntityId,
                   trigger_stage_id AS TriggerStageId,
                   overall_status AS OverallStatus,
                   requested_by AS RequestedBy,
                   reason AS Reason,
                   created_at AS CreatedAt,
                   updated_at AS UpdatedAt
            FROM approval_service.entity_approval_request
            WHERE entity_type = @EntityType AND entity_id = @EntityId
            ORDER BY created_at DESC;";

        var requests = (await connection.QueryAsync<EntityApprovalRequest>(
            new CommandDefinition(queryReqSql, new { EntityType = entityType, EntityId = entityId }, cancellationToken: ct)
        )).ToList();

        var result = new List<object>();
        foreach (var req in requests)
        {
            const string queryItemsSql = @"
                SELECT id_approval_item AS Id,
                       id_approval_request AS ApprovalRequestId,
                       id_approval_catalog AS ApprovalCatalogId,
                       department AS Department,
                       is_mandatory AS IsMandatory,
                       blocks_progress AS BlocksProgress,
                       blocks_commission AS BlocksCommission,
                       blocks_activation AS BlocksActivation,
                       blocks_liquidation AS BlocksLiquidation,
                       status AS Status,
                       comments AS Comments,
                       resolved_by AS ResolvedBy,
                       resolved_at AS ResolvedAt,
                       created_at AS CreatedAt,
                       updated_at AS UpdatedAt
                FROM approval_service.entity_approval_item
                WHERE id_approval_request = @RequestId
                ORDER BY id_approval_item ASC;";

            var items = await connection.QueryAsync<EntityApprovalItem>(
                new CommandDefinition(queryItemsSql, new { RequestId = req.Id }, cancellationToken: ct)
            );

            result.Add(new
            {
                request = req,
                items = items
            });
        }

        return Ok(result);
    }
}

public class CreateApprovalRequestDto
{
    public string EntityType { get; set; } = "Order";
    public string EntityId { get; set; } = string.Empty;
    public int TriggerStageId { get; set; }
    public int? RequestedBy { get; set; }
    public string? Reason { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class EvaluateTransitionDto
{
    public string EntityType { get; set; } = "Order";
    public string EntityId { get; set; } = string.Empty;
    public int ToStatusId { get; set; }
}

public class TransitionResultDto
{
    public bool CanTransition { get; set; }
    public List<BlockerDetailDto> Blockers { get; set; } = new();
}

public class BlockerDetailDto
{
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public class ResolveItemDto
{
    public int ApprovalItemId { get; set; }
    public string Status { get; set; } = string.Empty; // APPROVED or REJECTED
    public string? Comments { get; set; }
    public int? ResolvedBy { get; set; }
}
