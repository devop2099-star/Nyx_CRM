using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Entities;

namespace CRM.ApiHub.Domain.Repositories;

public interface ICheckpointRepository
{
    Task<IEnumerable<CheckpointCatalog>> GetCatalogAsync(CancellationToken ct = default);
    Task<IEnumerable<OrderCheckpoint>> GetByOrderAsync(long orderId, CancellationToken ct = default);
    Task<OrderCheckpoint?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<OrderCheckpointStepStatus>> GetStepStatusesAsync(int orderCheckpointId, CancellationToken ct = default);
    Task<bool> UpdateCheckpointStatusAsync(int id, string status, string? comments, int? checkedBy, CancellationToken ct = default);
    Task<bool> ToggleStepStatusAsync(int orderCheckpointId, int stepIndex, bool isCompleted, int? updatedBy, CancellationToken ct = default);
    Task<bool> TriggerCheckpointsForOrderAsync(long orderId, int triggerStageId, CancellationToken ct = default);
    Task<IEnumerable<OrderCheckpoint>> GetBlockingCheckpointsForTransitionAsync(long orderId, int toStageId, CancellationToken ct = default);
}
