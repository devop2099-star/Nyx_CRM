using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Application.Interfaces;

namespace CRM.ApiHub.Application.UseCases.Checkpoints;

public class UpdateCheckpointStatusUseCase
{
    private readonly ICheckpointRepository _checkpointRepository;
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly INotificationService _notificationService;
    private readonly IHttpClientFactory _httpClientFactory;

    public UpdateCheckpointStatusUseCase(
        ICheckpointRepository checkpointRepository,
        ISalesOrderRepository salesOrderRepository,
        INotificationService notificationService,
        IHttpClientFactory httpClientFactory)
    {
        _checkpointRepository = checkpointRepository;
        _salesOrderRepository = salesOrderRepository;
        _notificationService = notificationService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> ExecuteAsync(int id, string status, string? comments, int checkedBy, CancellationToken ct = default)
    {
        var checkpoint = await _checkpointRepository.GetByIdAsync(id, ct);
        if (checkpoint == null) return false;

        // Delegate status resolution to CheckpointEngine microservice
        var httpClient = _httpClientFactory.CreateClient("CheckpointEngine");
        var response = await httpClient.PostAsJsonAsync("api/engine/resolve-checkpoint", new
        {
            CheckpointId = id,
            Status = status,
            Comments = comments,
            CheckedBy = checkedBy
        }, ct);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<ResolveResultDto>(cancellationToken: ct);
        if (result == null || !result.Success)
        {
            return false;
        }

        // Apply Rollback locally if required and entity is Order
        if (result.RollbackRequired && string.Equals(result.EntityType, "Order", StringComparison.OrdinalIgnoreCase))
        {
            if (long.TryParse(result.EntityId, out long orderId))
            {
                var order = await _salesOrderRepository.GetByIdAsync(orderId, ct);
                if (order != null && result.RollbackStageId.HasValue)
                {
                    int rollbackStageId = result.RollbackStageId.Value;
                    var commentText = $"Retroceso automático de etapa debido al rechazo en checkpoint: {checkpoint.CheckpointName}. Comentarios: {comments}";
                    
                    // Update sales order status locally
                    await _salesOrderRepository.UpdateStatusAsync(
                        orderId,
                        rollbackStageId,
                        null,
                        commentText,
                        checkedBy,
                        false,
                        ct
                    );

                    // Notify owner
                    await _notificationService.SendNotificationAsync(
                        userId: order.IdUser,
                        title: "Checkpoint Rechazado - Orden Retornada",
                        message: $"Tu orden #{orderId} ha sido retornada a la etapa {rollbackStageId} debido a que el checkpoint '{checkpoint.CheckpointName}' fue rechazado.",
                        module: "SalesOrder",
                        actionData: orderId.ToString()
                    );
                }
            }
        }

        return true;
    }
}

public class ResolveResultDto
{
    public bool Success { get; set; }
    public bool RollbackRequired { get; set; }
    public int? RollbackStageId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
