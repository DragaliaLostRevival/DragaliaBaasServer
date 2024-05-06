using System.Text.Json.Serialization;

namespace DragaliaBaasServer.Models.Web;

public class SdkTokenRequest
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [JsonPropertyName("session_token")]
    public string SessionToken { get; set; } = string.Empty;
}