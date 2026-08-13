using System;

namespace Nyx.ApprovalEngine.Models;

public class EntityApprovalItem
{
    public int Id { get; set; }
    public int ApprovalRequestId { get; set; }
    public int? ApprovalCatalogId { get; set; }
    public string Department { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = true;
    public bool BlocksProgress { get; set; }
    public bool BlocksCommission { get; set; }
    public bool BlocksActivation { get; set; }
    public bool BlocksLiquidation { get; set; }
    public string Status { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED
    public string? Comments { get; set; }
    public int? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
