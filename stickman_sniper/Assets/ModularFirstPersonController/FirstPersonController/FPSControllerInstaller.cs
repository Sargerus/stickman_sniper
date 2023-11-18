using DWTools;
using UnityEngine;
using Zenject;

public class FPSControllerInstaller : MonoInstaller
{
    [SerializeField] private CameraProvider _fpsCamera;

    public override void InstallBindings()
    {
        Container.BindInstance(_fpsCamera).WithId(CameraProvider.WorldCamera).AsSingle();
        Container.BindInstance(gameObject.GetComponent<FirstPersonController>());
    }
}
