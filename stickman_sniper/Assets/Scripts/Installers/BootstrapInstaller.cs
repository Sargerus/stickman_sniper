using DWTools;
using YG;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<InputService>().AsSingle().WithArguments(YandexGame.Device);
    }
}
