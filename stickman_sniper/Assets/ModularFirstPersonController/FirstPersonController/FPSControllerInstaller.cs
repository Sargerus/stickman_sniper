using DWTools;
using UnityEngine;
using Zenject;

public class FPSControllerInstaller : MonoInstaller
{
    [SerializeField] private CameraProvider _fpsCamera;
    [SerializeField] private HandsController _handsController;

    public override void InstallBindings()
    {
        Container.BindInstance(_fpsCamera).WithId(CameraProvider.WorldCamera).AsSingle();
        Container.BindInstance(gameObject.GetComponent<FirstPersonController>());
        Container.BindInterfacesAndSelfTo<WeaponFactory>().AsSingle();

        Container.BindInterfacesTo<WeaponService>().AsSingle();
        Container.BindInterfacesTo<HandsController>().FromInstance(_handsController).AsSingle();
    }
}
