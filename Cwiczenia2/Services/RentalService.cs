using Cwiczenia2.Models;
using Cwiczenia2.Models.Devices;
using Cwiczenia2.Models.Users;

namespace Cwiczenia2.Services;

public record RentalResult(bool Success, string Message, Rental? Rental = null);

public class RentalService(DataStore store, DataService data)
{
    private readonly DataStore _store = store;
    private readonly DataService _data = data;

    public IReadOnlyList<Device> GetAllDevices() => _store.Devices.AsReadOnly();
    public IReadOnlyList<Device> GetAvailableDevices() =>
        _store.Devices.Where(d => d.Status == DeviceStatus.Available).ToList();

    public void AddDevice(Device device)
    {
        _store.Devices.Add(device);
        DataService.Save(_store);
    }

    public bool RemoveDevice(string deviceId)
    {
        var device = FindDevice(deviceId);
        if (device is null || device.Status == DeviceStatus.Rented) return false;

        _store.Devices.Remove(device);
        DataService.Save(_store);
        return true;
    }

    public bool SetDeviceMaintenance(string deviceId, bool underMaintenance)
    {
        var device = FindDevice(deviceId);
        if (device is null || device.Status == DeviceStatus.Rented) return false;

        device.Status = underMaintenance ? DeviceStatus.Unavailable : DeviceStatus.Available;
        DataService.Save(_store);
        return true;
    }

    public IReadOnlyList<User> GetAllUsers() => _store.Users.AsReadOnly();

    public void AddUser(User user)
    {
        _store.Users.Add(user);
        DataService.Save(_store);
    }

    public RentalResult Rent(string userId, string deviceId, int daysAllowed = 7)
    {
        var user   = _store.Users.FirstOrDefault(u => u.ID == userId);
        var device = FindDevice(deviceId);

        if (user   is null)
            return Fail("Fail: User not found.");
        if (device is null)
            return Fail("Fail: Device not found.");
        if (device.Status != DeviceStatus.Available)
            return Fail($"Fail: Device is currently {device.Status} and cannot be rented.");

        var activeCount = _store.Rentals.Count(
            r => r.UserID == userId && r.Status == RentalStatus.Active);

        if (activeCount >= user.MaxRentals)
            return Fail($"Fail: {user.Role} may hold at most {user.MaxRentals} active rentals.");

        var rental = new Rental
        {
            UserID   = userId,
            DeviceID = deviceId,
            DueDate  = DateTime.UtcNow.AddDays(daysAllowed),
        };

        device.Status = DeviceStatus.Rented;
        _store.Rentals.Add(rental);
        DataService.Save(_store);

        return new(true,
            $"OK: Rented '{device.Name}' to {user.Name} until {rental.DueDate:yyyy-MM-dd}.",
            rental);
    }

    public RentalResult Return(string rentalId)
    {
        var rental = _store.Rentals.FirstOrDefault(r => r.ID == rentalId);
        if (rental is null) return Fail("Fail: Rental was not found.");
        if (rental.Status != RentalStatus.Active) return Fail("Fail: Rental is not active.");

        var device = FindDevice(rental.DeviceID);
        if (device is not null) device.Status = DeviceStatus.Available;

        rental.ReturnedAt = DateTime.UtcNow;
        rental.Status = RentalStatus.Returned;
        DataService.Save(_store);

        return new(true, $"OK: '{device?.Name ?? rental.DeviceID}' returned successfully.");
    }

    public IReadOnlyList<Rental> GetAllRentals()    => _store.Rentals.AsReadOnly();
    public IReadOnlyList<Rental> GetActiveRentals() =>
        _store.Rentals.Where(r => r.Status == RentalStatus.Active).ToList();
    public IReadOnlyList<Rental> GetUserRentals(string userId) =>
        _store.Rentals.Where(r => r.UserID == userId).ToList();

    public int UpdateOverdueStatuses()
    {
        var overdue = _store.Rentals
            .Where(r => r.Status == RentalStatus.Active && DateTime.UtcNow > r.DueDate)
            .ToList();

        foreach (var r in overdue) r.Status = RentalStatus.Overdue;

        if (overdue.Count > 0) DataService.Save(_store);

        return overdue.Count;
    }

    private Device? FindDevice(string id) =>
        _store.Devices.FirstOrDefault(d => d.ID == id);

    private static RentalResult Fail(string msg) => new(false, msg);
}