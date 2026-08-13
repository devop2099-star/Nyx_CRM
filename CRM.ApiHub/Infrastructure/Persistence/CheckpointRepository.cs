using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;
using CRM.ApiHub.Domain.Repositories;
using Dapper;
using Microsoft.Extensions.Logging;

namespace CRM.ApiHub.Infrastructure.Persistence;

public class CheckpointRepository : ICheckpointRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<CheckpointRepository> _logger;

    public CheckpointRepository(IDbConnectionFactory connectionFactory, ILogger<CheckpointRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<CheckpointCatalog>> GetCatalogAsync(CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM checkpoint_service.checkpoint_catalog WHERE is_active = true ORDER BY id_checkpoint_catalog ASC;";
        return await connection.QueryAsync<CheckpointCatalog>(new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task<IEnumerable<OrderCheckpoint>> GetByOrderAsync(long orderId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT ec.id_entity_checkpoint AS Id,
                   ec.entity_type AS EntityType,
                   ec.entity_id AS EntityId,
                   ec.id_checkpoint_catalog AS CheckpointCatalogId,
                   ec.status AS Status,
                   ec.comments AS Comments,
                   ec.checked_by AS CheckedBy,
                   ec.checked_at AS CheckedAt,
                   ec.steps_completed AS StepsCompleted,
                   ec.created_at AS CreatedAt,
                   ec.updated_at AS UpdatedAt,
                   cc.name AS CheckpointName, 
                   cc.applies_to AS AppliesTo, 
                   cc.origin AS Origin, 
                   cc.scope AS Scope, 
                   cc.blocks_progress AS BlocksProgress, 
                   cc.blocks_commission AS BlocksCommission, 
                   cc.blocks_activation AS BlocksActivation, 
                   cc.blocks_liquidation AS BlocksLiquidation, 
                   cc.rollback_stage_id AS RollbackStageId, 
                   cc.owner_department AS OwnerDepartment, 
                   cc.required_steps_count AS RequiredStepsCount,
                   u.username AS CheckedByUsername
            FROM checkpoint_service.entity_checkpoint ec
            JOIN checkpoint_service.checkpoint_catalog cc ON ec.id_checkpoint_catalog = cc.id_checkpoint_catalog
            LEFT JOIN user_service.users u ON ec.checked_by = u.id_user
            WHERE ec.entity_type = 'Order' AND ec.entity_id = @OrderId::varchar
            ORDER BY ec.created_at ASC;";

        return await connection.QueryAsync<OrderCheckpoint>(new CommandDefinition(sql, new { OrderId = orderId }, cancellationToken: ct));
    }

    public async Task<OrderCheckpoint?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT ec.id_entity_checkpoint AS Id,
                   ec.entity_type AS EntityType,
                   ec.entity_id AS EntityId,
                   ec.id_checkpoint_catalog AS CheckpointCatalogId,
                   ec.status AS Status,
                   ec.comments AS Comments,
                   ec.checked_by AS CheckedBy,
                   ec.checked_at AS CheckedAt,
                   ec.steps_completed AS StepsCompleted,
                   ec.created_at AS CreatedAt,
                   ec.updated_at AS UpdatedAt,
                   cc.name AS CheckpointName, 
                   cc.applies_to AS AppliesTo, 
                   cc.origin AS Origin, 
                   cc.scope AS Scope, 
                   cc.blocks_progress AS BlocksProgress, 
                   cc.blocks_commission AS BlocksCommission, 
                   cc.blocks_activation AS BlocksActivation, 
                   cc.blocks_liquidation AS BlocksLiquidation, 
                   cc.rollback_stage_id AS RollbackStageId, 
                   cc.owner_department AS OwnerDepartment, 
                   cc.required_steps_count AS RequiredStepsCount,
                   u.username AS CheckedByUsername
            FROM checkpoint_service.entity_checkpoint ec
            JOIN checkpoint_service.checkpoint_catalog cc ON ec.id_checkpoint_catalog = cc.id_checkpoint_catalog
            LEFT JOIN user_service.users u ON ec.checked_by = u.id_user
            WHERE ec.id_entity_checkpoint = @Id;";

        return await connection.QueryFirstOrDefaultAsync<OrderCheckpoint>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<IEnumerable<OrderCheckpointStepStatus>> GetStepStatusesAsync(int orderCheckpointId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT @OrderCheckpointId AS OrderCheckpointId,
                   cs.step_index AS StepIndex,
                   COALESCE(os.is_completed, false) AS IsCompleted,
                   os.updated_by AS UpdatedBy,
                   os.updated_at AS UpdatedAt,
                   cs.description AS StepDescription
            FROM checkpoint_service.entity_checkpoint ec
            JOIN checkpoint_service.checkpoint_step cs ON ec.id_checkpoint_catalog = cs.id_checkpoint_catalog
            LEFT JOIN checkpoint_service.entity_checkpoint_step_status os ON ec.id_entity_checkpoint = os.id_entity_checkpoint AND cs.step_index = os.step_index
            WHERE ec.id_entity_checkpoint = @OrderCheckpointId
            ORDER BY cs.step_index ASC;";

        return await connection.QueryAsync<OrderCheckpointStepStatus>(new CommandDefinition(sql, new { OrderCheckpointId = orderCheckpointId }, cancellationToken: ct));
    }

    public async Task<bool> UpdateCheckpointStatusAsync(int id, string status, string? comments, int? checkedBy, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE checkpoint_service.entity_checkpoint
            SET status = @Status,
                comments = @Comments,
                checked_by = @CheckedBy,
                checked_at = @CheckedAt,
                updated_at = NOW()
            WHERE id_entity_checkpoint = @Id;";

        var rows = await connection.ExecuteAsync(new CommandDefinition(sql, new 
        { 
            Id = id, 
            Status = status, 
            Comments = comments, 
            CheckedBy = checkedBy,
            CheckedAt = checkedBy.HasValue ? (DateTime?)DateTime.UtcNow : null
        }, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<bool> ToggleStepStatusAsync(int orderCheckpointId, int stepIndex, bool isCompleted, int? updatedBy, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        if (connection.State != ConnectionState.Open) connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            // 1. Upsert step status
            const string upsertSql = @"
                INSERT INTO checkpoint_service.entity_checkpoint_step_status (id_entity_checkpoint, step_index, is_completed, updated_by, updated_at)
                VALUES (@OrderCheckpointId, @StepIndex, @IsCompleted, @UpdatedBy, NOW())
                ON CONFLICT (id_entity_checkpoint, step_index) 
                DO UPDATE SET is_completed = EXCLUDED.is_completed, updated_by = EXCLUDED.updated_by, updated_at = NOW();";

            await connection.ExecuteAsync(upsertSql, new 
            { 
                OrderCheckpointId = orderCheckpointId, 
                StepIndex = stepIndex, 
                IsCompleted = isCompleted, 
                UpdatedBy = updatedBy 
            }, transaction: transaction);

            // 2. Count completed steps
            const string countSql = @"
                SELECT COUNT(*) 
                FROM checkpoint_service.entity_checkpoint_step_status 
                WHERE id_entity_checkpoint = @OrderCheckpointId AND is_completed = true;";
            
            var completedCount = await connection.ExecuteScalarAsync<int>(countSql, new { OrderCheckpointId = orderCheckpointId }, transaction: transaction);

            // 3. Get checkpoint and required steps count
            const string checkSql = @"
                SELECT ec.status, cc.required_steps_count 
                FROM checkpoint_service.entity_checkpoint ec
                JOIN checkpoint_service.checkpoint_catalog cc ON ec.id_checkpoint_catalog = cc.id_checkpoint_catalog
                WHERE ec.id_entity_checkpoint = @OrderCheckpointId;";
            
            var checkData = await connection.QueryFirstOrDefaultAsync<dynamic>(checkSql, new { OrderCheckpointId = orderCheckpointId }, transaction: transaction);
            
            if (checkData != null)
            {
                int required = checkData.required_steps_count;
                string currentStatus = checkData.status;
                string newStatus = currentStatus;

                // Auto-resolve if all steps are completed
                if (required > 0)
                {
                    if (completedCount == required)
                    {
                        newStatus = "APROBADO";
                    }
                    else if (currentStatus == "APROBADO")
                    {
                        newStatus = "EN_PROCESO";
                    }
                    else if (currentStatus == "PENDIENTE")
                    {
                        newStatus = "EN_PROCESO";
                    }
                }

                const string updateCheckpointSql = @"
                    UPDATE checkpoint_service.entity_checkpoint
                    SET steps_completed = @CompletedCount,
                        status = @NewStatus,
                        checked_by = CASE WHEN @NewStatus = 'APROBADO' THEN @UpdatedBy ELSE checked_by END,
                        checked_at = CASE WHEN @NewStatus = 'APROBADO' THEN NOW() ELSE checked_at END,
                        updated_at = NOW()
                    WHERE id_entity_checkpoint = @OrderCheckpointId;";

                await connection.ExecuteAsync(updateCheckpointSql, new 
                { 
                    OrderCheckpointId = orderCheckpointId, 
                    CompletedCount = completedCount, 
                    NewStatus = newStatus,
                    UpdatedBy = updatedBy
                }, transaction: transaction);
            }

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling step status for checkpoint {OrderCheckpointId}, step {StepIndex}", orderCheckpointId, stepIndex);
            transaction.Rollback();
            return false;
        }
    }

    public async Task<bool> TriggerCheckpointsForOrderAsync(long orderId, int triggerStageId, CancellationToken ct = default)
    {
        // Enrutado al microservicio para evitar duplicación. Retorna verdadero para mantener firma del repositorio local
        return true;
    }

    public async Task<IEnumerable<OrderCheckpoint>> GetBlockingCheckpointsForTransitionAsync(long orderId, int toStageId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT ec.id_entity_checkpoint AS Id,
                   cc.name AS CheckpointName,
                   cc.owner_department AS OwnerDepartment
            FROM checkpoint_service.entity_checkpoint ec
            JOIN checkpoint_service.checkpoint_catalog cc ON ec.id_checkpoint_catalog = cc.id_checkpoint_catalog
            WHERE ec.entity_type = 'Order' AND ec.entity_id = @OrderId::varchar
              AND cc.blocks_progress = true
              AND ec.status NOT IN ('APROBADO', 'SUBSANADO');";

        return await connection.QueryAsync<OrderCheckpoint>(
            new CommandDefinition(sql, new { OrderId = orderId }, cancellationToken: ct)
        );
    }
}
