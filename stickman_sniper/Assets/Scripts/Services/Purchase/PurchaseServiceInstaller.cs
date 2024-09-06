using UnityEngine;
using Zenject;

namespace Purchase
{
    public class PurchaseServiceInstaller : MonoInstaller
    {
        [SerializeField] private HashToProductKeyMapper _hashToProductKeyMapper;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PurchaseService>().AsSingle().WithArguments(_hashToProductKeyMapper);
        }
    }
}