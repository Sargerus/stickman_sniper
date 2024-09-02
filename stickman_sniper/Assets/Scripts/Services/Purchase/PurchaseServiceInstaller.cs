using stickman_sniper.Purchases;
using UnityEngine;
using Zenject;

public class PurchaseServiceInstaller : MonoInstaller
{
    [SerializeField] private HashToProductKeyMapper _hashToProductKeyMapper;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PurchaseService>().AsSingle().WithArguments(_hashToProductKeyMapper);
    }
}
