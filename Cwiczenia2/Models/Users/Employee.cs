namespace Cwiczenia2.Models.Users;

public class Employee : User
{
    public string Department { get; set; } = string.Empty;
    public string Title { get; set; } = "inż.";
    public override string Role => string.Empty;
    public override int MaxRentals => 5;
}