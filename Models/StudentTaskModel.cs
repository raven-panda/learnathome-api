using System.ComponentModel.DataAnnotations;

namespace LearnAtHomeApi.Models;

public class StudentTaskModel
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int AttributedUserId { get; set; }
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(300)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [Required]
    public int CreatedByUserId { get; set; }
    public int UpdatedByUserId { get; set; }
    [Required]
    public DateTime EndDate { get; set; }

}