using Zenject;

public class SaveServiceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<SaveService>().AsSingle();
    }
}
