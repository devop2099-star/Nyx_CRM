using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CRM.ApiHub.Application.DTOs;
using CRM.ApiHub.Domain.Repositories;

namespace CRM.ApiHub.Application.UseCases.Checkpoints;

public class GetOrderCheckpointsUseCase
{
    private readonly ICheckpointRepository _repository;

    public GetOrderCheckpointsUseCase(ICheckpointRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<OrderCheckpointDto>> ExecuteAsync(long orderId, CancellationToken ct = default)
    {
        var checkpoints = await _repository.GetByOrderAsync(orderId, ct);
        var dtos = new List<OrderCheckpointDto>();

        foreach (var cp in checkpoints)
        {
            var steps = await _repository.GetStepStatusesAsync(cp.Id, ct);
            dtos.Add(new OrderCheckpointDto
            {
                Checkpoint = cp,
                Steps = steps
            });
        }

        return dtos;
    }
}
