namespace Cwiczenia2.Models;

public enum RentalStatus { Active, Returned, Overdue }
public class Rental
{
    private static readonly int PenaltyValue = 5;
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string UserID { get; set; } = string.Empty;
    public string DeviceID { get; set; } = string.Empty;
    public DateTime RentedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public DateTime? ReturnedAt { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Active;

    public bool IsOverdue =>
        Status == RentalStatus.Active && DateTime.UtcNow > DueDate;
}