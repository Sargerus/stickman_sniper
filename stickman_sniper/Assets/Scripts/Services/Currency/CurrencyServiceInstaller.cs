using Zenject;

namespace stickman_sniper.Currency
{
    public class CurrencyServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CurrencyService>().AsSingle();
        }
    }
}