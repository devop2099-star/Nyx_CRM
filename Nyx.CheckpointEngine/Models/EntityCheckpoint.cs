using System;

namespace Nyx.CheckpointEngine.Models;

public class EntityCheckpoint
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public int CheckpointCatalogId { get; set; }
    public string Status { get; set; } = "PENDIENTE";
    public string? Comments { get; set; }
    public int? CheckedBy { get; set; }
    public DateTime? CheckedAt { get; set; }
    public int StepsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Joined fields for convenience
    public string CheckpointName { get; set; } = string.Empty;
    public string AppliesTo { get; set; } = string.Empty;
    public string Origin { get; set; } = "INTERNO";
    public bool BlocksProgress { get; set; }
    public bool BlocksCommission { get; set; }
    public bool BlocksActivation { get; set; }
    public bool BlocksLiquidation { get; set; }
    public int? RollbackStageId { get; set; }
    public int? TriggerOnKoCatalogId { get; set; }
    public string OwnerDepartment { get; set; } = string.Empty;
    public int RequiredStepsCount { get; set; }
}
