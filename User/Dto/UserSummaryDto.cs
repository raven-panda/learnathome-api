using System.Text.Json.Serialization;

namespace LearnAtHomeApi.User.Dto;

public class UserSummaryDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }
}
