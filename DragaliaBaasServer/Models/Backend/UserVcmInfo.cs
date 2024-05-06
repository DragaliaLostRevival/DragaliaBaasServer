using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DragaliaBaasServer.Models.Vcm;

namespace DragaliaBaasServer.Models.Backend;

public class UserVcmInfo
{
    public VcmCurrency VirtualCurrencyName { get; set; } = VcmCurrency.diamond;
    public VcmMarket Market { get; set; } = VcmMarket.GOOGLE;

    [JsonIgnore]
    public List<UserVcmBalance> Balances { get; set; } = new();

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public string UserAccountId { get; set; } = string.Empty;

    [NotMapped]
    public List<UserVcmBalance> RemittedBalances => Balances.Where(balance => balance.Remitted).ToList();

    [NotMapped]
    public UserVcmBalance Balance => Balances.FirstOrDefault(balance => !balance.Remitted) ?? new UserVcmBalance();
}