using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class LevelInstaller : MonoInstaller, IPreloadBehavior
{
    [SerializeField] private AssetReference _characterPrefab;

    public override void InstallBindings()
    {
        GameObject characterGo = (GameObject)_characterPrefab.Asset;
        Character character = characterGo.GetComponent<Character>();

        Container.BindFactory<Character, Character.Factory>().FromSubContainerResolve().ByNewContextPrefab(character).AsSingle();
        Container.BindInterfacesTo<LevelProgressObserver>().AsSingle();
        
        Container.BindInterfacesTo<LevelConstructor>().AsSingle().NonLazy();
        Container.BindInitializableExecutionOrder<LevelConstructor>(int.MaxValue);
    }

    public async UniTask Load()
    {
        await _characterPrefab.LoadAssetAsync<GameObject>();
    }

    public async UniTask Clear()
    {
        _characterPrefab.ReleaseAsset();
    }
}
