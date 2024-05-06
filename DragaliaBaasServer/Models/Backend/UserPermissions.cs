using Microsoft.EntityFrameworkCore;

namespace DragaliaBaasServer.Models.Backend;

[Owned]
public class UserPermissions
{
    public bool PersonalAnalytics { get; set; } = false;
    public bool PersonalNotification { get; set; } = false;
    public ulong PersonalAnalyticsUpdatedAt { get; set; } = 0;
    public ulong PersonalNotificationUpdatedAt { get; set; } = 0;
}