namespace Cwiczenia2.Models.Devices;
 
public class Notebook : Device
{
    public string OS { get; set; } = "Ubuntu 21";
    public int RamGb { get; set; } = 8;
    public string CPU { get; set; } = string.Empty;
 
    public override string DeviceType => "Notebook";
}