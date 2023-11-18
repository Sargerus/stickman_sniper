using DWTools;
using YG;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<LevelLoader>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelLoader>(int.MaxValue);

        Container.BindInterfacesTo<InputService>().AsSingle().WithArguments(YandexGame.Device);
    }
}
