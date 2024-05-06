using DragaliaBaasServer.Models.Backend;

namespace DragaliaBaasServer.Models.Vcm;

public record VcmWallet(
    string UserId,
    VcmCurrency VirtualCurrencyName,
    VcmMarket Market,
    IEnumerable<UserVcmBalance> RemittedBalances,
    UserVcmBalance Balance
)
{
    public VcmWallet(string id, UserVcmInfo vcmInfo)
        : this(id, vcmInfo.VirtualCurrencyName, vcmInfo.Market, vcmInfo.RemittedBalances, vcmInfo.Balance) {}
};