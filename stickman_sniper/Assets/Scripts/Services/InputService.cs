using YG;

public enum Device
{
    None = 0,
    Mobile = 10,
    Desktop = 20,
    Tablet = 30
}

public interface IInputService
{
    Device Device { get; }
}

public class InputService : IInputService
{
    public Device Device { get; }

    public InputService()
    {
        Device = DeviceExtensions.StringToDevice(YandexGame.Device);
    }
}

public static class DeviceExtensions
{
    public static Device StringToDevice(string device) => device switch
    {
        "mobile" => Device.Mobile,
        "desktop" => Device.Desktop,
        "tablet" => Device.Tablet,
        _ => Device.Desktop
    };
}