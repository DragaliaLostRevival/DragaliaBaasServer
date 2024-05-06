namespace DragaliaBaasServer;

public static class Constants
{
#if DEBUG
    public const string ServerUrl = "https://baas-dev.lukefz.xyz";
#else
    public const string ServerUrl = "https://baas.lukefz.xyz";
#endif
}