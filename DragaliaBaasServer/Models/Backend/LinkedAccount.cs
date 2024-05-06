namespace DragaliaBaasServer.Models.Backend;

public class LinkedAccount : BaseEntity, IHasId
{
    public string Id { get; set; }
    public object? Userdata { get; set; }

    public LinkedAccount(string id)
    {
        Id = id;
    }
}