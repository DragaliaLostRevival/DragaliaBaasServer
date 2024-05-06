namespace DragaliaBaasServer.Models.Backend;

public abstract class BaseUser : BaseEntity, IHasId
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Birthday { get; set; } = "0000-00-00";
    public string Gender { get; set; } = "unknown";
}