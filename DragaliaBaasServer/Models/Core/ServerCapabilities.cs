namespace DragaliaBaasServer.Models.Core;

public class ServerCapabilities
{
#if DEBUG
    private const string Host = "baas-dev.lukefz.xyz";
#else
    private const string Host = "baas.lukefz.xyz";
#endif

    public string AccountHost { get; set; } = Host;
    public string AccountApiHost { get; set; } = Host;
    public string PointProgramHost { get; set; } = Host;
    public uint SessionUpdateInterval { get; set; }

    public ServerCapabilities(uint sessionUpdateInterval)
    {
        SessionUpdateInterval = sessionUpdateInterval;
    }
}