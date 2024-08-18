using Core.Entities.Concrete;
using System.Text.Json.Serialization;

namespace Core.Utilities.Security.JWT;

public class AccessToken
{
    [JsonPropertyName("Token")]
    public string Token { get; set; }

    [JsonPropertyName("Expiration")]
    public DateTime Expiration { get; set; }

    [JsonPropertyName("User")]
    public TokenUser User { get; set; }

    [JsonPropertyName("IsSuccessful")]
    public bool IsSuccessful { get; set; }

    [JsonPropertyName("IsVerified")]
    public bool IsVerified { get; set; }
}