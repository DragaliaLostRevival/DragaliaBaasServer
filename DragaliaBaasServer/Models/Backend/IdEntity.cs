namespace DragaliaBaasServer.Models.Backend;

public class IdEntity : IHasId
{
    public string Id { get; set; }

    public IdEntity() : this(string.Empty) {}

    public IdEntity(string id)
    {
        Id = id;
    }
}