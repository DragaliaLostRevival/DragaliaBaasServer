using DragaliaBaasServer.Models.Backend;

namespace DragaliaBaasServer.Models.Web;

public class SdkUser : BaseUser
{
    public bool AnalyticsOptedIn = false;
    public ulong AnalyticsOptedInUpdatedAt = 0;
    public SdkAnalyticsPermissions AnalyticsPermissions = new();
    public bool ClientFriendsOptedIn = false;
    public ulong ClientFriendsOptedInUpdatedAt = 0;
    public SdkEmailOptIn EachEmailOptedIn = new();
    public bool EmailOptedIn = false;
    public ulong EmailOptedInUpdatedAt = 0;
    public bool EmailVerified = true;
    public bool IsChild = false;
    public string Language = "en-US";
    public object? Region = null;
    public SdkTimezone Timezone = new();

    public SdkUser(WebUserAccount webUser)
    {
        Id = webUser.Id;
        Nickname = webUser.Username;
        Country = "US";
    }

    public class SdkAnalyticsPermissions
    {
        public SdkAnalyticsPermission InternalAnalysis = new();
        public SdkAnalyticsPermission TargetMarketing = new();
    }

    public class SdkAnalyticsPermission
    {
        public bool Permitted = false;
        public ulong UpdatedAt = 0;
    }

    public class SdkEmailOptIn
    {
        public SdkEmailOptInEntry Deals = new();
        public SdkEmailOptInEntry Survey = new();
    }

    public class SdkEmailOptInEntry
    {
        public bool OptedIn = false;
        public ulong UpdatedAt = 0;
    }

    public class SdkTimezone
    {
        public string Id = "America/New_York";
        public string Name = "America/New_York";
        public string UtcOffset = "-05:00";
        public long UtcOffsetSeconds = -18000;
    }
}