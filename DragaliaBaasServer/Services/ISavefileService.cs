using DragaliaBaasServer.Models.Web;

namespace DragaliaBaasServer.Services;

public interface ISavefileService
{
    public Task<bool> SaveSavefile(WebUserAccount webAccount, Stream saveFile);
    public Stream? GetSavefile(WebUserAccount webAccount);
    public bool DeleteSavefile(WebUserAccount webAccount);
}