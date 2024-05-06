namespace DragaliaBaasServer.Models.Vcm;

public record VcmBundle(
    VcmCurrency VirtualCurrencyName,
    VcmMarket Market,
    IEnumerable<VcmBundleItem> Items
)
{
    public static List<VcmBundle> GetEmptyBundlesForMarket(VcmMarket market)
        => new()
        {
            new VcmBundle(VcmCurrency.diamond, market, new List<VcmBundleItem>())
        };
};

public record VcmBundleItem(
    string Id,
    VcmCurrency VirtualCurrencyName,
    uint Amount,
    uint ExtraAmount,
    string Sku,
    double UsdPrice,
    double Price,
    string Title,
    string? Detail,
    bool Disabled,
    uint EventNotifyStartAt,
    object? CustomAttribute
);