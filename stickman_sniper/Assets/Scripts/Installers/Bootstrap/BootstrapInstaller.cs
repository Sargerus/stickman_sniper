using DWTools;
using UnityEngine;
using YG;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private TouchLocker _touchLocker;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<LevelLoader>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelLoader>(int.MaxValue);

        Container.Bind<TouchLocker>().FromInstance(_touchLocker);
        Container.Bind<IAudioManager>().FromInstance(_audioManager).AsSingle();
    }
}
