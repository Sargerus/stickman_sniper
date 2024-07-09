using Counters;
using DWTools;
using DWTools.Slowmotion;
using System;
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

        Container.BindInterfacesTo<LoadingManagerHolder>().AsSingle();

        Container.Bind<TouchLocker>().FromInstance(_touchLocker);
        Container.Bind<CursorLocker>().AsSingle().WithArguments(YandexGame.Device).NonLazy();
        Container.Bind<IAudioManager>().FromInstance(_audioManager).AsSingle();

        Container.BindInterfacesTo<GameStartWeaponInventoryService>().AsSingle().WithArguments<string, string>("Sniper 01", null);

        InstallFeatures();
        InstallAnalytics();

        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    }

    private void InstallFeatures()
    {
        Container.BindInterfacesTo<SlowmotionFeature>().AsSingle().NonLazy();
    }

    private void InstallAnalytics()
    {
        Container.BindInterfacesAndSelfTo<SessionCounter>().AsSingle().NonLazy();
    }
}
