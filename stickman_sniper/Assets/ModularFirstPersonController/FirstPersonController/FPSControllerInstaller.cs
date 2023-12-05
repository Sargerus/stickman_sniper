using DWTools;
using UnityEngine;
using YG;
using Zenject;

public class FPSControllerInstaller : MonoInstaller
{
    [SerializeField] private CameraProvider _fpsCamera;
    [SerializeField] private CameraProvider _uiCamera;
    [SerializeField] private CameraProvider _sniperCamera;
    [SerializeField] private CameraProvider _mobileCamera;
    [SerializeField] private CameraProvider _weaponCamera;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private HandsController _handsController;
    [SerializeField] private MobileCanvas _mobileCanvas;

    public override void InstallBindings()
    {
        Container.BindInstance(_fpsCamera).WithId(CameraProvider.WorldCamera).AsCached();
        Container.BindInstance(_uiCamera).WithId(CameraProvider.UICamera).AsCached();
        Container.BindInstance(_sniperCamera).WithId("sniper").AsCached();
        Container.BindInstance(_mobileCamera).WithId("mobile").AsCached();
        Container.BindInstance(_weaponCamera).WithId("weapon").AsCached();
        Container.Bind<FirstPersonController>().FromComponentOnRoot().AsSingle();
        Container.BindInterfacesAndSelfTo<WeaponFactory>().AsSingle();
        Container.Bind<IMobileInputProvider>().FromInstance(_mobileCanvas).AsSingle();
        Container.BindInterfacesTo<InputService>().AsSingle().WithArguments(YandexGame.Device);
        Container.BindInterfacesTo<WeaponService>().AsSingle();
        Container.BindInterfacesTo<HandsController>().FromInstance(_handsController).AsSingle();
        Container.BindInterfacesTo<PlayerProgressObserver>().AsSingle().NonLazy();
        Container.Bind<IUiManager>().FromInstance(_uiManager).AsSingle();
        Container.BindInterfacesTo<WinLoseDecider>().AsSingle().NonLazy();
    }
}
