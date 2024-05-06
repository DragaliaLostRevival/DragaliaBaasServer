using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DragaliaBaasServer.Models.Backend;

namespace DragaliaBaasServer.Models.Web;

public class WebUserAccount : IHasId
{
    [Required]
    [StringLength(255, ErrorMessage = "Invalid username. Must be at least 1 and at most 255 characters in size.", MinimumLength = 1)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(255, ErrorMessage = "Invalid password. Must be at least 8 and at most 255 characters in size.", MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [JsonIgnore]
    public string Id { get; set; } = string.Empty;

    [JsonIgnore]
    [NotMapped]
    public bool HasSavefile => SavefileUploadedAt != 0;

    [JsonIgnore]
    public long SavefileUploadedAt { get; set; }

    [JsonIgnore]
    [StringLength(32)]
    public string LinkedPatreonUserId { get; set; } = string.Empty;
}