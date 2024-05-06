namespace DragaliaBaasServer.Models.Analytics;

public class AnalyticsConfig
{
    public string AccessToken { get; } = "aaa";
    public string ApplicationId { get; } = "c6e6e04aaa8c635a";
    public string City { get; } = "aaa";
    public string Country { get; } = "US";
    public ulong ExpirationTime { get; } = long.MaxValue;
    public bool ImmediateReporting { get; } = false;
    public string Mode { get; } = "V2";
    public string Region { get; } = "eng";
    public ulong ReportingPeriod { get; } = 2147483000;
    public string Topic { get; } = "aaa";
}