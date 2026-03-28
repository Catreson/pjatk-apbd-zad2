using System.Text.Json;
using Cwiczenia2.Models;
using Cwiczenia2.Models.Devices;
using Cwiczenia2.Models.Users;

namespace Cwiczenia2.Services;

public class DataStore
{
    public List<Device> Devices { get; set; } = [];
    public List<User> Users { get; set; } = [];
    public List<Rental> Rentals { get; set; } = [];
}
public class DataService
{
    private static readonly string FilePath = 
        Path.Combine(AppContext.BaseDirectory, "data.json");

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
    };

    public DataStore Load()
    {
        if (!File.Exists(FilePath))
        {
            Console.WriteLine("[INFO] No data file found — seeding with demo data.");
            var fresh = SeedDefaults();
            Save(fresh);
            return fresh;
        }

        try
        {
            var json  = File.ReadAllText(FilePath);
            var store = JsonSerializer.Deserialize<DataStore>(json, Options);
            return store ?? SeedDefaults();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[WARN] Could not parse {FilePath}: {ex.Message}");
            return SeedDefaults();
        }
    }

    public static void Save(DataStore store)
    {
        var json = JsonSerializer.Serialize(store, Options);
        File.WriteAllText(FilePath, json);
    }
    private static DataStore SeedDefaults()
    {
        var store = new DataStore();

        store.Users.AddRange([
            new Administrator { Name = "System Admin", Email = "admin@pjwstk.edu.pl" },
            new Employee { Name = "Pap. Anna Kowalska", Email = "a.kowalska@pjwstk.edu.pl", Department = "Computer Science", Title = "Pap." },
            new Employee { Name = "Mag Marek Zając", Email = "m.zajac@pjwstk.edu.pl", Department = "Computer Magic", Title = "Mag" },
            new Student  { Name = "Jan Nowak", Email = "j.nowak@pjwstk.edu.pl", SkaNumber = "s216937", Faculty = "Informatics" },
            new Student  { Name = "Maria Wiśniewska", Email = "m.wisniewska@pjwstk.edu.pl", SkaNumber = "s1234", Faculty = "" },
        ]);

        store.Devices.AddRange([
            new Notebook { Name = "Dell XPS 15", RamGb = 16, CPU = "Intel i7-13th", OS = "Windows 11" },
            new Notebook { Name = "MacBook Pro 14\"", RamGb = 16, CPU = "Apple M3", OS = "macOS" },
            new Notebook { Name = "ThinkPad X1 Carbon", RamGb = 32, CPU = "Intel i7-12th", OS = "Ubuntu 24" },
            new Projector { Name = "Epson EB-U05", LumensOutput = 3400, ResolutionWidth = 1920, ResolutionHeight = 1080 },
            new Projector { Name = "BenQ MH560", LumensOutput = 4000, ResolutionWidth = 1920, ResolutionHeight = 1080 },
            new Camera { Name = "Canon EOS R50", MegaPixels = 24, Brand = "Canon", IncludesLens = true },
            new Camera { Name = "Sony A7 IV", MegaPixels = 33, Brand = "Sony", IncludesLens = false },
            new Mouse { Name = "Razer DeathAdder", Brand = "Razer", Dpi = 600009, LeftHand = false },
        ]);

        return store;
    }
}