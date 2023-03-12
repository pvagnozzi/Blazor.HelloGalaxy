using System.Text.Json.Serialization;

namespace Blazor.HelloGalaxy.Shared;

public record SignInRespose
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;
}

