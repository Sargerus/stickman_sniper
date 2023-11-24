using DWTools;
using UnityEngine;
using YG;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private test_lock test_Lock;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<LevelLoader>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelLoader>(int.MaxValue);

        //test_Lock.Construct(Container.Resolve<LevelLoader>());
    }
}
