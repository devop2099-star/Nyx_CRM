using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.ApiHub.Domain.Entities;

[Table("entity_checkpoint_step_status", Schema = "checkpoint_service")]
public class OrderCheckpointStepStatus
{
    [Column("id_entity_checkpoint")]
    public int OrderCheckpointId { get; set; }

    [Column("step_index")]
    public int StepIndex { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Join fields
    [NotMapped]
    public string StepDescription { get; set; } = string.Empty;
}
