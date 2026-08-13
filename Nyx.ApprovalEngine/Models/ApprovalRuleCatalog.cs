using System;

namespace Nyx.ApprovalEngine.Models;

public class ApprovalRuleCatalog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AppliesTo { get; set; } = "Toda campaña nueva";
    public int TriggerStageId { get; set; }
    public string RequiredDepartment { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = true;
    public bool BlocksProgress { get; set; }
    public bool BlocksCommission { get; set; }
    public bool BlocksActivation { get; set; }
    public bool BlocksLiquidation { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
