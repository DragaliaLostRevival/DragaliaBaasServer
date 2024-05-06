using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DragaliaBaasServer.Models.Vcm;
using DragaliaBaasServer.Models.Web;

namespace DragaliaBaasServer.Models.Backend;

public class UserAccount : BaseUser
{
    public UserPermissions Permissions { get; set; } = new();
    public bool HasUnreadCsComment { get; set; }

    [JsonIgnore]
    public string? WebUserAccountId { get; set; }

    [JsonIgnore]
    public WebUserAccount? WebUserAccount { get; set; }

    [JsonIgnore]
    public ExtendedUserInfo ExtendedUserInfo { get; set; } = new();

    [JsonIgnore]
    public List<UserVcmInfo> VcmInfo { get; set; } = new(); // Enum.GetValues<VcmMarket>().Select(market => new UserVcmInfo(market)).ToList()

    [JsonIgnore]
    public List<DeviceAccount> AssociatedDeviceAccounts { get; set; } = new();

    [NotMapped]
    public LinkedAccountInfo? Links => WebUserAccountId != null ? new LinkedAccountInfo(WebUserAccountId) : null;

    [NotMapped]
    public IEnumerable<IdEntity> DeviceAccounts
        => AssociatedDeviceAccounts.Select(dAccount => new IdEntity(dAccount.Id));

    public IEnumerable<VcmWallet> GetVcmWalletsForMarket(VcmMarket market)
        => VcmInfo.Where(info => info.Market == market).Select(info => new VcmWallet(Id, info));

    public UserInquiryInfo GetInquiryInfo()
        => new(Id, HasUnreadCsComment, UpdatedAt);
}