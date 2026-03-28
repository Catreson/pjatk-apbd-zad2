namespace Cwiczenia2.Models.Users;

public class Student : User
{
    public string SkaNumber { get; set; } = string.Empty;
    public string Faculty     { get; set; } = string.Empty;
    public override string Role => "Student";
    public override int MaxRentals => 2;
}