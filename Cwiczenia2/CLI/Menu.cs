using Cwiczenia2.Models.Devices;
using Cwiczenia2.Models.Users;
using Cwiczenia2.Services;

namespace Cwiczenia2.CLI;

public class Menu(RentalService svc)
{
    private readonly RentalService _svc = svc;

    public void Run()
    {
        var overdue = _svc.UpdateOverdueStatuses();
        if (overdue > 0)
            Warn($"Warning {overdue} rentals marked overdue.");

        while (true)
        {
            Console.WriteLine("Press enter to continue");
            Console.ReadLine()?.Trim();

            Console.WriteLine();
            Header("PJATK Device Rental");
            Console.WriteLine("  1  List all devices");
            Console.WriteLine("  2  List all users");
            Console.WriteLine("  3  Rent a device");
            Console.WriteLine("  4  Return a device");
            Console.WriteLine("  5  Show active rentals");
            Console.WriteLine("  6  Add device");
            Console.WriteLine("  7  Add user");
            Console.WriteLine("  8  Disable/enable device");
            Console.WriteLine("  0  Exit");
            Console.Write("\nChoice › ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    ListDevices();
                    break;
                case "2":  
                    ListUsers();
                    break;
                case "3":
                    DoRent();
                    break;
                case "4":
                    DoReturn();
                    break;
                case "5":
                    ListActiveRentals();
                    break;
                case "6":
                    AddDevice();
                    break;
                case "7":
                    AddUser();
                    break;
                case "8":
                    ToggleDevice();
                    break;
                case "0":
                    return;
                default:  Warn("Unknown option — try again."); break;
            }
        }
    }


    private void ListDevices()
    {
        Header("All devices");
        var devices = _svc.GetAllDevices();
        if (!devices.Any()) { Console.WriteLine("  (none)"); return; }

        foreach (var d in devices)
        {
            var color = d.Status switch
            {
                DeviceStatus.Available => ConsoleColor.Green,
                DeviceStatus.Rented => ConsoleColor.Yellow,
                DeviceStatus.Unavailable => ConsoleColor.DarkRed,
                _ => ConsoleColor.Gray
            };
            Print($"  {d}", color);
        }
    }

    private void ListUsers()
    {
        Header("All users");
        foreach (var u in _svc.GetAllUsers())
            Console.WriteLine($"  {u}");
    }

    private void ListActiveRentals()
    {
        Header("Active rentals");
        var userMap = _svc.GetAllUsers()  .ToDictionary(u => u.ID, u => u.Name);
        var deviceMap = _svc.GetAllDevices().ToDictionary(d => d.ID, d => d.Name);

        var active = _svc.GetActiveRentals();
        if (!active.Any()) {
            Console.WriteLine("  no active rentals");
            return;
        }

        foreach (var r in active)
        {
            var user = userMap.GetValueOrDefault(r.UserID,   r.UserID[..8]);
            var device = deviceMap.GetValueOrDefault(r.DeviceID, r.DeviceID[..8]);
            var tag = r.IsOverdue ? "  OVERDUE" : string.Empty;
            var color = r.IsOverdue ? ConsoleColor.Red : ConsoleColor.Gray;
            Print($"  [{r.ID[..8]}]  {user,-20} → {device,-28} due {r.DueDate:yyyy-MM-dd}{tag}", color);
        }
    }

    private void DoRent()
    {
        ListUsers();
        var user = PickById("User ID prefix", _svc.GetAllUsers());
        if (user is null) return;

        ListDevices();
        var device = PickById("Device ID prefix", _svc.GetAllDevices().Where(d => d.Status == DeviceStatus.Available));
        if (device is null) return;

        var result = _svc.Rent(user.ID, device.ID);
        (result.Success ? (Action<string>)Ok : Warn)(result.Message);
    }

    private void DoReturn()
    {
        ListActiveRentals();
        var rental = PickById("Rental ID prefix", _svc.GetActiveRentals());
        if (rental is null) return;

        var result = _svc.Return(rental.ID);
        (result.Success ? (Action<string>)Ok : Warn)(result.Message);
    }

    private void AddDevice()
    {
        Console.WriteLine("\n  Device type:  1-Notebook  2-Projector  3-Camera  4-Mouse");
        Console.Write("  Type › ");

        Device? d = Console.ReadLine()?.Trim() switch
        {
            "1" => new Notebook(),
            "2" => new Projector(),
            "3" => new Camera(),
            "4" => new Mouse(),
            _   => null
        };

        if (d is null) { 
            Warn("Unknown type.");
            return;
        }

        d.Name  = Prompt("Name");
        d.Notes = Prompt("Notes (optional)");

        if (d is Notebook l)
        {
            l.CPU = Prompt("CPU");
            l.OS = Prompt("OS");
            if (int.TryParse(Prompt("RAM (GB)"), out var ram)) l.RamGb = ram;
        }
        if (d is Camera c)
        {
            c.Brand = Prompt("Brand");
            if (int.TryParse(Prompt("Megapixels"), out var mp)) c.MegaPixels = mp;
        }
        if (d is Mouse m)
        {
            m.Brand = Prompt("Brand");
            if (int.TryParse(Prompt("DPI"), out var dpi)) m.Dpi = dpi;
        }

        _svc.AddDevice(d);
        Ok($"Device '{d.Name}' added (ID: {d.ID[..8]}).");
    }

    private void AddUser()
    {
        Console.WriteLine("\n  Role: 1-Student 2-Employee 3-Adminisrator");
        Console.Write("  Role › ");

        User? u = Console.ReadLine()?.Trim() switch
        {
            "1" => new Student(),
            "2" => new Employee(),
            "3" => new Administrator(),
            _   => null
        };

        if (u is null) {
            Warn("Fail: Unknown role.");
            return;
        }

        u.Name  = Prompt("Name");
        u.Email = Prompt("Email");

        if (u is Student  s) {
            s.SkaNumber = Prompt("Ska number");
            s.Faculty = Prompt("Faculty");
        }
        if (u is Employee p) {
            p.Title = Prompt("Title ");
            p.Department = Prompt("Department");
        }

        _svc.AddUser(u);
        Ok($"Ok: User '{u.Name}' ({u.Role}) added (ID: {u.ID[..8]}).");
    }

    private void ToggleDevice()
    {
        ListDevices();
        var device = PickById("Device ID prefix", _svc.GetAllDevices());
        if (device is null) return;

        var result = _svc.Toggle(device.ID);
        (result.Success ? (Action<string>)Ok : Warn)(result.Message);

    }
    private static T? PickById<T>(string prompt, IEnumerable<T> collection)
        where T : class
    {
        Console.Write($"\n  {prompt} › ");
        var prefix = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(prefix)) {
            Warn("Warn: Cancelled.");
            return null;
        }

        var prop = typeof(T).GetProperty("ID")!;
        var match = collection.FirstOrDefault(
            item => (prop.GetValue(item) as string)?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) == true);

        if (match is null) Warn($"Warn: No {typeof(T).Name} found with that prefix.");
        return match;
    }

    private static string Prompt(string label)
    {
        Console.Write($"  {label} › ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static void Header(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"── {title} ──");
        Console.ResetColor();
    }

    private static void Ok(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private static void Print(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
}