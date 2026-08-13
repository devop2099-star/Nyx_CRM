using System;

namespace Nyx.CheckpointEngine.Models;

public class CheckpointCatalog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AppliesTo { get; set; } = string.Empty;
    public int TriggerStageId { get; set; }
    public string Origin { get; set; } = "INTERNO";
    public string Scope { get; set; } = "Venta";
    public bool BlocksProgress { get; set; }
    public bool BlocksCommission { get; set; }
    public bool BlocksActivation { get; set; }
    public bool BlocksLiquidation { get; set; }
    public int? RollbackStageId { get; set; }
    public int? TriggerOnKoCatalogId { get; set; }
    public string OwnerDepartment { get; set; } = string.Empty;
    public int RequiredStepsCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
