using stickman_sniper.Purchases;
using Zenject;

public class PurchaseServiceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PurchaseService>().AsSingle();
    }
}
