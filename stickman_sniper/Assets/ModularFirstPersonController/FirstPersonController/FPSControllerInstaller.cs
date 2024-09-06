using DWTools;
using InfimaGames.LowPolyShooterPack;
using stickman_sniper.Producer;
using UnityEngine;
using YG;
using Zenject;

public class FPSControllerInstaller : MonoInstaller
{
    [SerializeField] private CameraProvider _fpsCamera;
    [SerializeField] private CameraProvider _uiCamera;
    [SerializeField] private CameraProvider _mobileCamera;
    [SerializeField] private CameraProvider _slowmotionCamera;
    [SerializeField] private MobileCanvas _mobileCanvas;

    public override void InstallBindings()
    {
        Container.BindInstance(_fpsCamera).WithId(CameraProvider.WorldCamera).AsCached();
        Container.BindInstance(_uiCamera).WithId(CameraProvider.UICamera).AsCached();
        Container.BindInstance(_mobileCamera).WithId("mobile").AsCached();
        Container.BindInstance(_slowmotionCamera).WithId("slowmotion").AsCached();
        Container.Bind<Character>().FromComponentOnRoot().AsSingle();
        Container.Bind<IMobileInputProvider>().FromInstance(_mobileCanvas).AsSingle();
        Container.BindInterfacesTo<InputService>().AsSingle().WithArguments(YandexGame.EnvironmentData.deviceType);
        Container.BindInterfacesTo<PlayerProgressObserver>().AsSingle().NonLazy();
        Container.BindInterfacesTo<WinLoseDecider>().AsSingle().NonLazy();
        Container.BindInterfacesTo<BulletFlyProducer>().AsSingle();
        Container.BindInterfacesTo<EnemyDeadProducer>().AsSingle();
        Container.BindInterfacesTo<CoreProducer>().AsSingle();
    }
}
