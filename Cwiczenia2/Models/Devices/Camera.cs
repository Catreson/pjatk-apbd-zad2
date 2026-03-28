namespace Cwiczenia2.Models.Devices;
 
public class Camera : Device
{
    public string Brand { get; set; } = string.Empty;
    public int MegaPixels { get; set; } = 24;
    public bool IncludesLens { get; set; } = true;
 
    public override string DeviceType => "Camera";
}