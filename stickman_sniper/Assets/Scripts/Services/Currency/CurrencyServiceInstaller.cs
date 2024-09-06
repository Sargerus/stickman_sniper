using Zenject;

namespace Currency
{
    public class CurrencyServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CurrencyService>().AsSingle();
        }
    }
}