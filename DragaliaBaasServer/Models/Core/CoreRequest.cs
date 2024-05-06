using DragaliaBaasServer.Models.Backend;

namespace DragaliaBaasServer.Models.Core;

public class CoreRequest
{
    public required string AppVersion { get; set; }
    public required string Assertion { get; set; }
    public string? Carrier { get; set; }
    public DeviceAccount? DeviceAccount { get; set; }
    public string? DeviceAnalyticsId { get; set; }
    public required string DeviceName { get; set; }
    public required string Locale { get; set; }
    public required string Manufacturer { get; set; }
    public required string NetworkType { get; set; }
    public required string OsType { get; set; }
    public required string OsVersion { get; set; }
    public required string SdkVersion { get; set; }
    public required string TimeZone { get; set; }
    public long TimeZoneOffset { get; set; }
    public string? SessionId { get; set; }
    public string? PreviousUserId { get; set; }
    public IdentityProviderAccount? IdpAccount { get; set; }
}