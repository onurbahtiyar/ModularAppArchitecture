using System.Text.Json.Serialization;

namespace Core.Entities.Concrete;

public class TokenUser
{
    [JsonPropertyName("UserId")]
    public int UserId { get; set; }
    [JsonPropertyName("Guid")]
    public Guid Guid { get; set; }

    [JsonPropertyName("Username")]
    public string Username { get; set; }

    [JsonPropertyName("Email")]
    public string Email { get; set; }

    [JsonPropertyName("FirstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("LastName")]
    public string LastName { get; set; }
    [JsonPropertyName("Roles")]
    public List<string> Roles { get; set; }
}