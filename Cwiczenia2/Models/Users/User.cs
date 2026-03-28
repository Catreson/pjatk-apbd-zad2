using System.Text.Json.Serialization;

namespace Cwiczenia2.Models.Users;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Student), typeDiscriminator: "Student")]
[JsonDerivedType(typeof(Employee), typeDiscriminator: "Employee")]
[JsonDerivedType(typeof(Administrator), typeDiscriminator: "Administrator")]
public abstract class User
{
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [JsonIgnore] public abstract string Role { get; }
    [JsonIgnore] public abstract int MaxRentals { get; }

    public override string ToString() =>
        $"[{Role,-10}] {Name,-25} {Surname,-25} <{Email,-35}> ID:{ID[..8]}";
}