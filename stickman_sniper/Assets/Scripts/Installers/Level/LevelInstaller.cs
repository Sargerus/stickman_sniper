using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField] private FirstPersonController _fpsPrefab;

    public override void InstallBindings()
    {
        Container.BindFactory<FirstPersonController, FirstPersonController.Factory>().FromSubContainerResolve().ByNewContextPrefab(_fpsPrefab).AsSingle();
        Container.BindInterfacesTo<LevelProgressObserver>().AsSingle();
        
        Container.BindInterfacesTo<LevelConstructor>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelConstructor>(int.MaxValue);
    }
}
