using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField] private Character _characterPrefab;

    public override void InstallBindings()
    {
        Container.BindFactory<Character, Character.Factory>().FromSubContainerResolve().ByNewContextPrefab(_characterPrefab).AsSingle();
        Container.BindInterfacesTo<LevelProgressObserver>().AsSingle();
        
        Container.BindInterfacesTo<LevelConstructor>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelConstructor>(int.MaxValue);
    }
}
