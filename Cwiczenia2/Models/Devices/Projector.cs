namespace Cwiczenia2.Models.Devices;

public class Projector : Device
{
    public int ResolutionWidth { get; set; } = 1920;
    public int ResolutionHeight { get; set; } = 1080;
    public int LumensOutput { get; set; } = 3000;

    public override string DeviceType => "Projector";
}