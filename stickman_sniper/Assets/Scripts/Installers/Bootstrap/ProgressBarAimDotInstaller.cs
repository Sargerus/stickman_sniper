using Zenject;

public class ProgressBarAimDotInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<ProgressBarAimDotProvider>().AsSingle().NonLazy();
    }
}
