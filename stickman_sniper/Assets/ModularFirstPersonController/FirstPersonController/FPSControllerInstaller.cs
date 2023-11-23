using DWTools;
using UnityEngine;
using YG;
using Zenject;

public class FPSControllerInstaller : MonoInstaller
{
    [SerializeField] private CameraProvider _fpsCamera;
    [SerializeField] private CameraProvider _uiCamera;
    [SerializeField] private HandsController _handsController;

    public override void InstallBindings()
    {
        Container.BindInstance(_fpsCamera).WithId(CameraProvider.WorldCamera).AsCached();
        Container.BindInstance(_uiCamera).WithId(CameraProvider.UICamera).AsCached();
        Container.Bind<FirstPersonController>().FromComponentOnRoot().AsSingle();
        Container.BindInterfacesAndSelfTo<WeaponFactory>().AsSingle();
        Container.BindInterfacesTo<InputService>().AsSingle().WithArguments(YandexGame.Device);
        Container.BindInterfacesTo<WeaponService>().AsSingle();
        Container.BindInterfacesTo<HandsController>().FromInstance(_handsController).AsSingle();
    }
}
