using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace DragaliaBaasServer.Models.Backend;

[Owned]
public class ExtendedUserInfo
{
    public UserStatus Status { get; set; } = UserStatus.None;
    public bool HasUploadedSaveData { get; set; } = false;

    [MemberNotNullWhen(true, nameof(IsBanned))]
    public string? BanReason { get; set; }

    [MemberNotNullWhen(true, nameof(IsTempBanned))]
    public DateTimeOffset? BanExpiration { get; set; }

    [MemberNotNullWhen(true, nameof(HasUploadedSaveData))]
    public string? SaveDataDownloadUrl { get; set; }

    [NotMapped]
    [JsonIgnore]
    internal bool IsBanned => Status != UserStatus.None;

    [NotMapped]
    [JsonIgnore]
    internal bool IsTempBanned => Status == UserStatus.TemporaryBan;
}