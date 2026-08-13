using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.ApiHub.Domain.Entities;

[Table("checkpoint_step", Schema = "checkpoint_service")]
public class CheckpointStep
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_checkpoint_step")]
    public int Id { get; set; }

    [Column("id_checkpoint_catalog")]
    public int CheckpointCatalogId { get; set; }

    [Column("step_index")]
    public int StepIndex { get; set; }

    [Column("description")]
    public string Description { get; set; } = string.Empty;
}
