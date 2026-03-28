namespace Cwiczenia2.Models.Users;

public class Administrator : User
{
    public override string Role => "Administrator";
    public override int MaxRentals => 7;
}