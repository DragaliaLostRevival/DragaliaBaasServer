namespace DragaliaBaasServer.Models.Web;

public record SdkTokenResponse(
    string AccessToken,
    string IdToken,
    string SessionToken,
    SdkUser User,
    uint ExpiresIn,
    object? Error = null,
    object? TermsAgreement = null
);