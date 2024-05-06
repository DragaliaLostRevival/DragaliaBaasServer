using System.Text.Json.Serialization;

namespace DragaliaBaasServer.Models.Web;

public class SessionTokenInfo
{
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("session_token")]
    public string SessionToken { get; set; } = string.Empty;
}