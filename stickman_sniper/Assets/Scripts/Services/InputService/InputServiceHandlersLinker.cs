namespace DWTools
{
    public interface IInputServiceHandlersLinker
    {
        void SetIsReloading(bool isReloading);
        void SetIsAiming(bool isAiming);
        void SetIsShooting(bool isShooting);
    }

    public partial class InputService : IInputServiceHandlersLinker
    {
        public void SetIsAiming(bool isAiming)
        {
            IsAiming = true;
        }

        public void SetIsReloading(bool isReloading)
        {
            IsReloading = isReloading;
        }

        public void SetIsShooting(bool isShooting)
        {
            IsShooting = isShooting;
        }
    }
}