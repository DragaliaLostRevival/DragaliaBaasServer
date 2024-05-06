using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;
using DragaliaBaasServer.Database.Repositories;
using DragaliaBaasServer.Models.Web;

namespace DragaliaBaasServer.Services;

public class PatreonService
{
    private readonly HttpClient _client;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<PatreonService> _logger;

    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly byte[] _stateEncryptionKey;
    private readonly byte[] _stateHmacKey;

    public PatreonService(HttpClient client, 
        IAccountRepository accountRepository,
        IConfiguration configuration,
        ILogger<PatreonService> logger)
    {
        _client = client;
        _accountRepository = accountRepository;
        _logger = logger;

        _clientId = configuration["Patreon:ClientId"]!;
        _clientSecret = configuration["Patreon:ClientSecret"]!;
        _stateEncryptionKey = Convert.FromHexString(configuration["Patreon:StateEncryptionKey"]!);
        _stateHmacKey = Convert.FromHexString(configuration["Patreon:StateHmacKey"]!);
    }

    public string BuildPatreonAuthUrl(WebUserAccount account)
    {
        var state = $"{account.Id}|{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}|";
        var hmac = Convert.ToHexString(HMACSHA1.HashData(_stateHmacKey, Encoding.UTF8.GetBytes(state)));
        var stateWithHmac = state + hmac;

        using var aes = Aes.Create();
        aes.Key = _stateEncryptionKey;
        var iv = RandomNumberGenerator.GetBytes(16);

        var encryptedState =
            Convert.ToBase64String(iv.Concat(aes.EncryptCbc(Encoding.UTF8.GetBytes(stateWithHmac), iv)).ToArray());

        return
            $"https://www.patreon.com/oauth2/authorize" +
            $"?response_type=code" +
            $"&client_id={_clientId}" +
            $"&redirect_uri={HttpUtility.UrlEncode($"{Constants.ServerUrl}/patreon/auth")}" +
            $"&scope=users" +
            $"&state={HttpUtility.UrlEncode(encryptedState)}";
    }

    private const string TokenUrl = "https://www.patreon.com/api/oauth2/token";

    private async Task<(string AccessToken, string RefreshToken)> RedeemCodeForToken(string code)
    {
        var oauthRequest = new HttpRequestMessage(HttpMethod.Post, TokenUrl);
        oauthRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"code", code},
            {"grant_type", "authorization_code"},
            {"client_id", _clientId},
            {"client_secret", _clientSecret},
            {"redirect_uri", $"{Constants.ServerUrl}/patreon/auth"}
        });

        var oauthResponse = await _client.SendAsync(oauthRequest);
        oauthResponse.EnsureSuccessStatusCode();

        var resp = (await oauthResponse.Content.ReadFromJsonAsync<TokenResponse>())!;

        return (resp.AccessToken, resp.RefreshToken);
    }

    public async Task<PatreonLinkResult> LinkPatreonAccount(string code, string state)
    {
        using var aes = Aes.Create();
        aes.Key = _stateEncryptionKey;
        var encryptedState = Convert.FromBase64String(state);
        var iv = encryptedState[..16];

        var decryptedState = Encoding.UTF8.GetString(aes.DecryptCbc(encryptedState[16..], iv));

        var endOfState = decryptedState.LastIndexOf('|') + 1;
        var plainState = decryptedState[..endOfState];
        var stateHmac = Convert.FromHexString(plainState[endOfState..]);
        if (HMACSHA1.HashData(_stateHmacKey, Encoding.UTF8.GetBytes(state)).SequenceEqual(stateHmac))
            return PatreonLinkResult.UnknownError;

        var userId = plainState.Split('|')[0];
        var account = _accountRepository.GetWebUserAccount(userId);

        if (account == null)
            return PatreonLinkResult.UnknownError;

        var (access, _) = await RedeemCodeForToken(code);

        const string currentUserUrl = "https://www.patreon.com/api/oauth2/api/current_user";

        var req = new HttpRequestMessage(HttpMethod.Get, currentUserUrl);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access);

        var resp = await _client.SendAsync(req);
        resp.EnsureSuccessStatusCode();

        var respContent = (await resp.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>())!;

        var patreonUserId = respContent.Data.Id;
        if (_accountRepository.DoesWebUserAccountWithPatreonIdExist(patreonUserId))
            return PatreonLinkResult.AlreadyLinked;

        account.LinkedPatreonUserId = patreonUserId;
        await _accountRepository.SaveChangesAsync();

        return PatreonLinkResult.Success;
    }

    private record TokenResponse(
        [property:JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("scope")] string Scope,
        [property: JsonPropertyName("token_type")] string TokenType
    );

    private record ApiResponse<T>(T Data);

    private record UserResponse(string Id);
}

public enum PatreonLinkResult
{
    Success,
    UnknownError,
    AlreadyLinked
}