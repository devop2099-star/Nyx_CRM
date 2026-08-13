using System;

namespace Nyx.ApprovalEngine.Models;

public class EntityApprovalRequest
{
    public int Id { get; set; }
    public string EntityType { get; set; } = "Order";
    public string EntityId { get; set; } = string.Empty;
    public int TriggerStageId { get; set; }
    public string OverallStatus { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED
    public int? RequestedBy { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
