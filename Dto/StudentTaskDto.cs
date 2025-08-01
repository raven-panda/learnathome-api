using System.ComponentModel.DataAnnotations;

namespace LearnAtHomeApi.Dto;

public class StudentTaskDto
{
    public int? Id { get; set; }
    [Required]
    public int AttributedUserId { get; set; }
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(300)]
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public int CreatedByUserId { get; init; }
    public int UpdatedByUserId { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
}