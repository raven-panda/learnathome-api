using System.ComponentModel.DataAnnotations;
using LearnAtHomeApi.User.Dto;

namespace LearnAtHomeApi.StudentTask.Dto;

public class StudentTaskDto
{
    public int Id { get; set; }

    [Required]
    public int AttributedUserId { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public UserSummaryDto CreatedBy { get; init; } = null!;
    public UserSummaryDto UpdatedBy { get; set; } = null!;

    [Required]
    public DateTime EndDate { get; set; }

    public int MentorId { get; set; }
}

public class CreateStudentTaskDto
{
    [Required]
    public int AttributedUserId { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime EndDate { get; set; }
}

public class UpdateStudentTaskDto : CreateStudentTaskDto
{
    public int Id { get; set; }
}
