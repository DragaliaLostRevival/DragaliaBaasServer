using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DragaliaBaasServer.Models.Backend;

public class DeviceAccount : IEquatable<DeviceAccount>, IHasId
{
    public string Id { get; set; }
    public string Password { get; set; }

    public DeviceAccount() : this(string.Empty, string.Empty) { }

    public DeviceAccount(string id, string password)
    {
        Id = id;
        Password = password;
    }

    public bool Equals(DeviceAccount? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && Password == other.Password;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((DeviceAccount) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Password);
    }
}