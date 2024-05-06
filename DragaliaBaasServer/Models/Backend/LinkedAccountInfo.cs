namespace DragaliaBaasServer.Models.Backend;

public class LinkedAccountInfo
{
    public LinkedAccount NintendoAccount { get; set; }

    public LinkedAccountInfo(string linkedAccountId)
    {
        NintendoAccount = new LinkedAccount(linkedAccountId);
    }
}