using Microsoft.EntityFrameworkCore;

namespace DragaliaBaasServer.Models.Vcm;

[Owned]
public class VcmBalancePaidEntry
{
    public string Code { get; set; } = string.Empty;
    public int Total { get; set; }
}