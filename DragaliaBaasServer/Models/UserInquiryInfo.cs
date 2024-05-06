namespace DragaliaBaasServer.Models;

public record UserInquiryInfo(
    string UserId,
    bool HasUnreadCsComment,
    ulong UpdatedAt
);