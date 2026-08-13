using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.ApiHub.Domain.Entities;

[Table("entity_checkpoint", Schema = "checkpoint_service")]
public class OrderCheckpoint
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_entity_checkpoint")]
    public int Id { get; set; }

    [Column("entity_type")]
    public string EntityType { get; set; } = "Order";

    [Column("entity_id")]
    public string EntityId { get; set; } = string.Empty;

    [NotMapped]
    public int OrderId 
    { 
        get => int.TryParse(EntityId, out int id) ? id : 0; 
        set => EntityId = value.ToString(); 
    }

    [Column("id_checkpoint_catalog")]
    public int CheckpointCatalogId { get; set; }

    [Column("status")]
    public string Status { get; set; } = "PENDIENTE";

    [Column("comments")]
    public string? Comments { get; set; }

    [Column("checked_by")]
    public int? CheckedBy { get; set; }

    [Column("checked_at")]
    public DateTime? CheckedAt { get; set; }

    [Column("steps_completed")]
    public int StepsCompleted { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Joined Fields (For API output convenience, not mapped in DB directly by EF if used, but used by Dapper)
    [NotMapped]
    public string CheckpointName { get; set; } = string.Empty;

    [NotMapped]
    public string AppliesTo { get; set; } = string.Empty;

    [NotMapped]
    public string Origin { get; set; } = "INTERNO";

    [NotMapped]
    public string Scope { get; set; } = "Venta";

    [NotMapped]
    public bool BlocksProgress { get; set; }

    [NotMapped]
    public bool BlocksCommission { get; set; }

    [NotMapped]
    public bool BlocksActivation { get; set; }

    [NotMapped]
    public bool BlocksLiquidation { get; set; }

    [NotMapped]
    public int? RollbackStageId { get; set; }

    [NotMapped]
    public int? TriggerOnKoCatalogId { get; set; }

    [NotMapped]
    public string OwnerDepartment { get; set; } = string.Empty;

    [NotMapped]
    public int RequiredStepsCount { get; set; }
    
    [NotMapped]
    public string? CheckedByUsername { get; set; }
}
