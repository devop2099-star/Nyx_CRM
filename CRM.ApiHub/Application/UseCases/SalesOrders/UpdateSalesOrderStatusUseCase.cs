using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Domain.Repositories;
using CRM.ApiHub.Application.Interfaces;

namespace CRM.ApiHub.Application.UseCases.SalesOrders;

public class UpdateSalesOrderStatusUseCase
{
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly INotificationService _notificationService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICampaignRepository _campaignRepository;

    public UpdateSalesOrderStatusUseCase(
        ISalesOrderRepository salesOrderRepository,
        INotificationService notificationService,
        IHttpClientFactory httpClientFactory,
        ICampaignRepository campaignRepository)
    {
        _salesOrderRepository = salesOrderRepository;
        _notificationService = notificationService;
        _httpClientFactory = httpClientFactory;
        _campaignRepository = campaignRepository;
    }

    public async Task<bool> ExecuteAsync(long idOrder, SalesOrderUpdateStatusDto dto, long actorId, CancellationToken ct = default)
    {
        var existingOrder = await _salesOrderRepository.GetByIdAsync(idOrder, ct);
        if (existingOrder == null) return false;

        // Si la orden está en estado >= 3 (EN BACKOFFICE en adelante) y el actor no es el usuario en custodia ni posee el rol correspondiente, rechazar la modificación.
        if (existingOrder.IdStatus >= 3 && existingOrder.CustodyUserId.HasValue && existingOrder.CustodyUserId.Value != actorId)
        {
            throw new InvalidOperationException($"Transición de estado no permitida. La orden #{idOrder} se encuentra en custodia del usuario {existingOrder.CustodyUserId.Value}.");
        }

        // Check for progress-blocking checkpoints via CheckpointEngine microservice
        var httpClient = _httpClientFactory.CreateClient("CheckpointEngine");
        var evalResponse = await httpClient.PostAsJsonAsync("api/engine/evaluate-transition", new 
        { 
            EntityType = "Order", 
            EntityId = idOrder.ToString(), 
            ToStatusId = dto.ToStatusId 
        }, ct);
        
        if (evalResponse.IsSuccessStatusCode)
        {
            var result = await evalResponse.Content.ReadFromJsonAsync<TransitionResultDto>(cancellationToken: ct);
            if (result != null && !result.CanTransition)
            {
                var names = string.Join(", ", result.Blockers.Select(b => $"'{b.Name}' ({b.Department})"));
                throw new InvalidOperationException($"Transición de estado bloqueada. Hay checkpoints pendientes de resolución: {names}.");
            }
        }

        // Check for progress-blocking approvals via ApprovalEngine microservice
        var approvalHttpClient = _httpClientFactory.CreateClient("ApprovalEngine");
        var appEvalResponse = await approvalHttpClient.PostAsJsonAsync("api/engine/evaluate-approval-transition", new 
        { 
            EntityType = "Order", 
            EntityId = idOrder.ToString(), 
            ToStatusId = dto.ToStatusId 
        }, ct);

        if (appEvalResponse.IsSuccessStatusCode)
        {
            var appResult = await appEvalResponse.Content.ReadFromJsonAsync<TransitionResultDto>(cancellationToken: ct);
            if (appResult != null && !appResult.CanTransition)
            {
                var names = string.Join(", ", appResult.Blockers.Select(b => $"'{b.Name}' ({b.Department})"));
                throw new InvalidOperationException($"Transición de estado bloqueada. Hay aprobaciones obligatorias pendientes: {names}.");
            }
        }

        var success = await _salesOrderRepository.UpdateStatusAsync(
            idOrder,
            dto.ToStatusId,
            dto.ToSubstatusId,
            dto.Comment,
            actorId,
            dto.IsBulk,
            ct
        );

        if (success)
        {
            // Trigger any checkpoints associated with the new status via CheckpointEngine microservice
            string campaignName = string.Empty;
            if (existingOrder.IdCmpg > 0)
            {
                var cmp = await _campaignRepository.GetByIdAsync((int)existingOrder.IdCmpg);
                if (cmp != null) campaignName = cmp.Name;
            }

            var metadata = new System.Collections.Generic.Dictionary<string, object>
            {
                { "campaign", campaignName }
            };

            await httpClient.PostAsJsonAsync("api/engine/trigger-checkpoints", new 
            { 
                EntityType = "Order", 
                EntityId = idOrder.ToString(), 
                ToStatusId = dto.ToStatusId,
                Metadata = metadata
            }, ct);

            // Trigger any approval requests associated with the new status via ApprovalEngine microservice
            await approvalHttpClient.PostAsJsonAsync("api/engine/create-request", new 
            { 
                EntityType = "Order", 
                EntityId = idOrder.ToString(), 
                TriggerStageId = dto.ToStatusId,
                RequestedBy = actorId,
                Reason = dto.Comment,
                Metadata = metadata
            }, ct);

            var order = await _salesOrderRepository.GetByIdAsync(idOrder, ct);
            if (order != null)
            {
                // Notify the original asesor about the status change
                await _notificationService.SendNotificationAsync(
                    userId: order.IdUser,
                    title: "Estado de Orden Actualizado",
                    message: $"El estado de tu orden #{idOrder} ha cambiado al estado {dto.ToStatusId}.",
                    module: "SalesOrder",
                    actionData: idOrder.ToString()
                );

                // When sent to Supervisor for revision (Status 2), notify custody supervisor
                const int SUPERVISOR_STATUS_ID = 2;
                if (dto.ToStatusId == SUPERVISOR_STATUS_ID)
                {
                    long targetSupervisorId = order.CustodyUserId.HasValue && order.CustodyUserId.Value != order.IdUser
                        ? order.CustodyUserId.Value
                        : 9; // Default Supervisor ID fallback (cnaranjo)

                    await _notificationService.SendNotificationAsync(
                        userId: targetSupervisorId,
                        title: $"Nueva Orden #{idOrder} lista para revisión",
                        message: $"El asesor ha enviado la orden #{idOrder} para revisión.",
                        module: "SUPERVISOR_REVISION",
                        actionData: idOrder.ToString()
                    );
                }

                // When sent to BackOffice individually, also notify the custody holder
                const int BACKOFFICE_STATUS_ID = 3;
                if (dto.ToStatusId == BACKOFFICE_STATUS_ID && order.CustodyUserId.HasValue && order.CustodyUserId.Value != order.IdUser)
                {
                    await _notificationService.SendNotificationAsync(
                        userId: order.CustodyUserId.Value,
                        title: $"Orden #{idOrder} asignada para revisión BAC",
                        message: $"Se te ha asignado la orden #{idOrder} para revisión desde el Supervisor.",
                        module: "TRANSFER",
                        actionData: idOrder.ToString()
                    );
                }
            }
        }

        return success;
    }
}

public class TransitionResultDto
{
    public bool CanTransition { get; set; }
    public System.Collections.Generic.List<BlockerDetailDto> Blockers { get; set; } = new();
}

public class BlockerDetailDto
{
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}
