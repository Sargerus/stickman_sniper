using DWTools.Slowmotion;
using Zenject;

public class AiSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<SlowmotionFeature>().AsSingle().NonLazy();
    }
}
