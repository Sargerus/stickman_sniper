using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField] private GameObject _fpsPrefab;

    public override void InstallBindings()
    {
        Container.BindFactory<FirstPersonController, FirstPersonController.Factory>().FromSubContainerResolve().ByNewContextPrefab(_fpsPrefab).AsSingle();
        
        Container.BindInterfacesAndSelfTo<LevelConstructor>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelConstructor>(int.MaxValue);
    }
}
