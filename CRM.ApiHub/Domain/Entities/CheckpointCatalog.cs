using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.ApiHub.Domain.Entities;

[Table("checkpoint_catalog", Schema = "checkpoint_service")]
public class CheckpointCatalog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_checkpoint_catalog")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("applies_to")]
    public string AppliesTo { get; set; } = string.Empty;

    [Column("trigger_stage_id")]
    public int TriggerStageId { get; set; }

    [Column("origin")]
    public string Origin { get; set; } = "INTERNO";

    [Column("scope")]
    public string Scope { get; set; } = "Venta";

    [Column("blocks_progress")]
    public bool BlocksProgress { get; set; }

    [Column("blocks_commission")]
    public bool BlocksCommission { get; set; }

    [Column("blocks_activation")]
    public bool BlocksActivation { get; set; }

    [Column("blocks_liquidation")]
    public bool BlocksLiquidation { get; set; }

    [Column("rollback_stage_id")]
    public int? RollbackStageId { get; set; }

    [Column("trigger_on_ko_catalog_id")]
    public int? TriggerOnKoCatalogId { get; set; }

    [Column("owner_department")]
    public string OwnerDepartment { get; set; } = string.Empty;

    [Column("required_steps_count")]
    public int RequiredStepsCount { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
