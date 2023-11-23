using DWTools;
using UnityEngine;
using YG;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    //[SerializeField] private GameObject _fpsPrefab;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<LevelLoader>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelLoader>(int.MaxValue);

        //Container.BindFactory<FirstPersonController, FirstPersonController.Factory>().FromSubContainerResolve().ByNewContextPrefab(_fpsPrefab).AsSingle();

        
        //Container.BindInterfacesAndSelfTo<testsuka>().AsSingle().NonLazy();
    }
}
