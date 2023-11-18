using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using YG;
using Zenject;

namespace DWTools
{
    public enum Device
    {
        None = 0,
        Mobile = 10,
        Desktop = 20,
        Tablet = 30
    }

    public interface IInputService
    {
        bool IsAiming { get; }
        bool IsReloading { get; }
        bool IsShooting { get; }
        Device Device { get; }

        void AddHandler(IInputHandler handler);
    }

    public partial class InputService : IInputService, ITickable, ILateTickable
    {
        private List<IInputHandler> _handlers = new();

        public Device Device { get; }

        #region Manager variables
        public bool IsReloading { get; private set; }
        public bool IsAiming { get; private set; }

        public bool IsShooting { get; private set; }
        #endregion

        public InputService()
        {
            Device = DeviceExtensions.StringToDevice(YandexGame.Device);
        }

        public void Tick()
        {
            _handlers.ForEach(g => g.Update());
        }

        public void LateTick()
        {
            _handlers.ForEach(g => g.LateUpdate());
        }

        public void AddHandler(IInputHandler handler)
        {
            if(_handlers.Contains(handler)) 
                return;

            handler.Link(this);
            _handlers.Add(handler);
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
}