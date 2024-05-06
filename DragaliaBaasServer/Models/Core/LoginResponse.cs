using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Vcm;

namespace DragaliaBaasServer.Models.Core;

public record LoginResponse(
    string IdToken,
    string AccessToken,
    UserAccount User,
    DeviceAccount? CreatedDeviceAccount,
    string? SessionId,
    uint ExpiresIn,
    VcmMarket? Market,
    ServerCapabilities Capability,
    object BehaviorSettings
);