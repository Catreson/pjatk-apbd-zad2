namespace Cwiczenia2.Models.Devices;

public class Mouse : Device
{
    public int Dpi { get; set; } = 6400;
    public string Brand { get; set; } = string.Empty;
    public bool LeftHand { get; set; } = false;

    public override string DeviceType => "Mouse";
}