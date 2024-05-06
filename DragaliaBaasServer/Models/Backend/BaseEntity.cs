namespace DragaliaBaasServer.Models.Backend;

public abstract class BaseEntity
{
    public ulong CreatedAt { get; set; }
    public ulong UpdatedAt { get; set; }
}