using System.Diagnostics.CodeAnalysis;
using DragaliaBaasServer.Models;
using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Core;
using DragaliaBaasServer.Models.Jwk;
using DragaliaBaasServer.Models.Web;
using Microsoft.AspNetCore.Components.Authorization;

namespace DragaliaBaasServer.Services;

public interface IAuthorizationService
{
    public string GenerateToken(IHasId user, AuthorizationTokenType tokenType, Dictionary<string, object>? additionalParameters = null);
    public bool TryParseToken(string token, [NotNullWhen(true)] out string? userId, bool validateLifetime = true);
    public bool TryLogin(UserAccount user, DeviceAccount? createdDeviceAccount, [NotNullWhen(true)] out LoginResponse? response);
    public JwkSet GetJwks();
    public uint GetTokenLifetime(AuthorizationTokenType type);
    public string GenerateRedirectUri(string webUserId, string redirectUriPrefix, string code, string state);
    public bool TryRedeemSessionTokenCode(string stcJwt, string stcCode, [NotNullWhen(true)] out string? webUserId);
    public Task<AuthenticationState> BuildAuthenticationState(WebUserAccount webAccount);
}