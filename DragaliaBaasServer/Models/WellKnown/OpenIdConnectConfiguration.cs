using System.Text.Json.Serialization;

namespace DragaliaBaasServer.Models.WellKnown;

public class OpenIdConnectConfiguration
{
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; init; } = string.Empty;

    [JsonPropertyName("id_token_signing_algs_supported")]
    public IEnumerable<string> IdTokenSigningAlgValuesSupported { get; init; } = Enumerable.Empty<string>();

    [JsonPropertyName("claims_supported")] 
    public IEnumerable<string> ClaimsSupported { get; init; } = Enumerable.Empty<string>();
}