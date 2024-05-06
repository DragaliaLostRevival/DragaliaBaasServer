namespace DragaliaBaasServer.Models.Core;

public class IdentityProviderAccount
{
    public required string IdToken { get; set; }
    public required string Idp { get; set; }
}