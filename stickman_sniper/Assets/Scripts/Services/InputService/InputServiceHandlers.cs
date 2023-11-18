using UnityEngine;

namespace DWTools
{
    public interface IInputHandler
    {
        public abstract void Link(IInputServiceHandlersLinker linker);
        public abstract void Update();
        public abstract void LateUpdate();
    }

    internal class ReloadingDesktopHandler : IInputHandler
    {
        private IInputServiceHandlersLinker _linker;

        public void Link(IInputServiceHandlersLinker linker)
        {
            _linker = linker;
        }

        public void LateUpdate()
        {
            _linker.SetIsReloading(false);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _linker.SetIsReloading(true);
            }
        }
    }

    internal class AimingDesktopHandler : IInputHandler
    {
        private IInputServiceHandlersLinker _linker;

        public void Link(IInputServiceHandlersLinker linker)
        {
            _linker = linker;
        }

        public void LateUpdate()
        {
            _linker.SetIsAiming(false);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _linker.SetIsAiming(true);
            }
        }
    }

    internal class ShootingDesktopHandler : IInputHandler
    {
        private IInputServiceHandlersLinker _linker;
        private bool _isShooting;

        public void Link(IInputServiceHandlersLinker linker)
        {
            _linker = linker;
        }

        public void LateUpdate()
        {
            if (!Input.GetKey(KeyCode.Mouse0) && _isShooting)
            {
                _linker.SetIsShooting(false);
                _isShooting = false;
            }
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _linker.SetIsShooting(true);
                _isShooting = true;
            }
        }
    }
}