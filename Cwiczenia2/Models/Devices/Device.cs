using System.Text.Json.Serialization;

namespace Cwiczenia2.Models.Devices;

public enum DeviceStatus { 
    Available,
    Rented,
    Unavailable
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Notebook), typeDiscriminator: "Laptop")]
[JsonDerivedType(typeof(Projector), typeDiscriminator: "Projector")]
[JsonDerivedType(typeof(Camera),    typeDiscriminator: "Camera")]
[JsonDerivedType(typeof(Mouse),    typeDiscriminator: "Mouse")]
public abstract class Device
{
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DeviceStatus Status { get; set; } = DeviceStatus.Available;

    [JsonIgnore]
    public abstract string DeviceType { get; }

    public override string ToString() =>
        $"[{DeviceType,-10}] {Name,-30} {Status,-12} ID:{ID[..8]}";
}