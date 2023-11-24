using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField] private FirstPersonController _fpsPrefab;
    [SerializeField] private Transform _envTransform;

    public override void InstallBindings()
    {
        //Container.BindInstance(_envTransform);

        Container.BindFactory<FirstPersonController, FirstPersonController.Factory>().FromSubContainerResolve().ByNewContextPrefab(_fpsPrefab).AsSingle();
        
        Container.BindInterfacesTo<LevelConstructor>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelConstructor>(int.MaxValue);
    }
}
