using System.ComponentModel.DataAnnotations;
using LearnAtHomeApi.User.Model;

namespace LearnAtHomeApi.StudentTask.Model;

public class StudentTaskModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AttributedUserId { get; set; }

    [Required]
    public RpUserModel Mentor { get; set; } = null!;

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public RpUserModel CreatedBy { get; set; } = null!;
    public RpUserModel UpdatedBy { get; set; } = null!;

    [Required]
    public DateTime EndDate { get; set; }
}
