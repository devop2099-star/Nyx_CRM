using System.Collections.Generic;
using CRM.ApiHub.Domain.Entities;

namespace CRM.ApiHub.Application.DTOs;

public class OrderCheckpointDto
{
    public OrderCheckpoint Checkpoint { get; set; } = null!;
    public IEnumerable<OrderCheckpointStepStatus> Steps { get; set; } = System.Array.Empty<OrderCheckpointStepStatus>();
}
