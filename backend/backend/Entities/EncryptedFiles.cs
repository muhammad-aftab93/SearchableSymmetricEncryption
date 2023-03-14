using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Entities;

[Table("EncryptedFiles")]
public class EncryptedFiles
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string FileName { get; set; } = null!;
    public string Content { get; set; } = null!;
}