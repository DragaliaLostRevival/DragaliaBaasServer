using DragaliaBaasServer.Models;
using DragaliaBaasServer.Models.Core;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using DragaliaBaasServer.Models.WellKnown;

namespace DragaliaBaasServer.Services;

public class AuthorizationService : IAuthorizationService
{
    private static readonly OpenIdConnectConfiguration OpenIdConnectConfiguration = new()
    {
        JwksUri = Constants.ServerUrl + "/.well-known/jwks.json",
        ClaimsSupported = new[]
        {
            "sub", "iss", "exp", "aud", "sav:a", "sav:ts", "p:id"
        },
        IdTokenSigningAlgValuesSupported = new[]
        {
            "S256"
        },
    };
        
    private readonly ILogger _logger;

    private readonly string _jwtIssuer;
    private readonly Dictionary<AuthorizationTokenType, uint> _tokenLifetimes;

    private readonly JwtSecurityTokenHandler _jwtHandler;
    private readonly TokenValidationParameters _validationParamters;
    private readonly JwtHeader _jwtHeader;
    private readonly JwkSet _keySet;
    private readonly string _jwtKid;

    public AuthorizationService(ILoggerFactory logger, IConfiguration configuration)
    {
        _logger = logger.CreateLogger("Auth");

        _jwtHandler = new JwtSecurityTokenHandler();
        _jwtIssuer = configuration["Jwt:Issuer"]!;
        _jwtKid = configuration["Jwt:Kid"]!;
        _tokenLifetimes = new Dictionary<AuthorizationTokenType, uint>();

        foreach (var val in Enum.GetNames(typeof(AuthorizationTokenType)))
        {
            _tokenLifetimes.Add(Enum.Parse<AuthorizationTokenType>(val), uint.Parse(configuration[$"ExpirationTimes:{val}"]!));
        }

        var privateRsa = RSA.Create();
        privateRsa.ImportFromPem(Encoding.UTF8.GetString(Convert.FromBase64String(configuration["Jwt:PrivateKey"]!)));
        var jwtPrivateKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(privateRsa));
        var jwtPublicKey = new RsaSecurityKey(privateRsa.ExportParameters(false));

        _keySet = new JwkSet();

        var publicJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(jwtPublicKey);
        publicJwk.Alg = "RS256";
        publicJwk.Use = "sig";
        publicJwk.Kid = _jwtKid;
        _keySet.Keys.Add(new Jwk(publicJwk));

        jwtPrivateKey.Kid = _jwtKid;
        var credentials = new SigningCredentials(jwtPrivateKey, SecurityAlgorithms.RsaSha256);

        _jwtHeader = new JwtHeader(credentials);

        _validationParamters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidIssuer = _jwtIssuer,
            IssuerSigningKey = jwtPrivateKey
        };
    }

    public string GenerateToken(IHasId user, AuthorizationTokenType tokenType, Dictionary<string, object>? additionalParameters = null)
    {
        var time = DateTime.UtcNow;
        var expiry = time.AddSeconds(_tokenLifetimes[tokenType]);
        var claims = new Dictionary<string, object> {{JwtRegisteredClaimNames.Sub, user.Id}};

        if (additionalParameters != null)
            foreach (var pair in additionalParameters)
                claims.TryAdd(pair.Key, pair.Value);

        var payload = new JwtPayload(_jwtIssuer, $"baas-{tokenType}", null, claims, time, expiry, time);

        return _jwtHandler.WriteToken(new JwtSecurityToken(_jwtHeader, payload));
    }

    public bool TryParseToken(string token, [NotNullWhen(true)] out string? userId, bool validateLifetime = true)
    {
        userId = null;

        var jwt = DecodeToken(token, validateLifetime);

        if (jwt != null)
        {
            userId = jwt.Subject;
            return true;
        }

        return false;
    }

    public JwtSecurityToken? DecodeToken(string token, bool validateLifetime = true)
    {
        try
        {
            _validationParamters.ValidateLifetime = validateLifetime;
            _jwtHandler.ValidateToken(token, _validationParamters, out var secToken);

            if (secToken is JwtSecurityToken jwtToken)
                return jwtToken;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to parse JWT token. Exception: {ex}", ex);
        }
        finally
        {
            _validationParamters.ValidateLifetime = true;
        }

        return null;
    }

    public bool TryLogin(UserAccount user, DeviceAccount? createdDeviceAccount, [NotNullWhen(true)] out LoginResponse? response)
    {
        var additionalParameters = new Dictionary<string, object>
        {
            {"sav:a", user.WebUserAccount?.HasSavefile ?? false},
            {"sav:ts", user.WebUserAccount?.SavefileUploadedAt ?? 0},
            {"p:id", user.WebUserAccount?.LinkedPatreonUserId ?? ""}
        };

        var idToken = GenerateToken(user, AuthorizationTokenType.Id, additionalParameters);
        var accessToken = GenerateToken(user, AuthorizationTokenType.Access);

        var lifetime = _tokenLifetimes[AuthorizationTokenType.Access];
        var capabilities = new ServerCapabilities(lifetime);

        response = new LoginResponse(
            idToken,
            accessToken,
            user,
            createdDeviceAccount,
            "",
            lifetime,
            null,
            capabilities,
            new object()
        );

        return true;
    }

    public JwkSet GetJwks() => _keySet;

    public uint GetTokenLifetime(AuthorizationTokenType type) => _tokenLifetimes[type];

    public string GenerateRedirectUri(string webUserId, string redirectUriPrefix, string code, string state)
    {
        var sb = new StringBuilder();
        sb.Append(redirectUriPrefix);
        sb.Append("#session_state=deadbeef");
        sb.Append("&session_token_code=");

        var tokenParams = new Dictionary<string, object> {{"stc:c", code}};

        sb.Append(GenerateToken(new WebUserAccount {Id = webUserId}, AuthorizationTokenType.Session, tokenParams));
        sb.Append("&state=");
        sb.Append(state);

        return sb.ToString();
    }

    public bool TryRedeemSessionTokenCode(string stcJwt, string stcCode, [NotNullWhen(true)] out string? webUserId)
    {
        webUserId = null;

        var stc = DecodeToken(stcJwt);
        if (stc == null
            || !stc.Payload.TryGetValue("stc:c", out var stcC)
            || stcC is not string actualStcCodeHash
            || stc.Audiences.First() != $"baas-{AuthorizationTokenType.Session}")
        {
            _logger.LogInformation("Tried to redeem session token but provided invalid stc JWT.");
            return false;
        }

        var submittedStcCodeHash = SHA256.HashData(Encoding.UTF8.GetBytes(stcCode));
        if (!submittedStcCodeHash.SequenceEqual(Base64UrlEncoder.DecodeBytes(actualStcCodeHash)))
        {
            _logger.LogInformation("Tried to redeem session token but provided invalid code.");
            return false;
        }

        webUserId = stc.Subject;

        return true;
    }

    public Task<AuthenticationState> BuildAuthenticationState(WebUserAccount webAccount)
    {
        var principal = new ClaimsPrincipal();
        var identity = new ClaimsIdentity("BaasWebAuth");

        var nameClaim = new Claim(ClaimTypes.Name, webAccount.Username);
        var idClaim = new Claim(ClaimTypes.NameIdentifier, webAccount.Id);

        identity.AddClaim(nameClaim);
        identity.AddClaim(idClaim);

        if (webAccount.Username == "LukeFZ")
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
        }

        principal.AddIdentity(identity);
        return Task.FromResult(new AuthenticationState(principal));
    }

    public OpenIdConnectConfiguration GetOpenIdConnectConfiguration()
    {
        return OpenIdConnectConfiguration;
    }
}