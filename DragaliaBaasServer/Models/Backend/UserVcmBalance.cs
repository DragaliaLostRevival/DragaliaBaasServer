using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DragaliaBaasServer.Models.Vcm;

namespace DragaliaBaasServer.Models.Backend;

public class UserVcmBalance
{
    public ulong Free { get; set; }
    public List<VcmBalancePaidEntry> Paid { get; set; } = new();
    public ulong Total { get; set; }

    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public bool Remitted { get; set; }
}