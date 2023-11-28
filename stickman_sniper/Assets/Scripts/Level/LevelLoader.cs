using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

public interface ILevelLoader
{
    UniTask LoadLevel();
}

public class LevelLoader : IInitializable, ILevelLoader
{
    public async UniTask LoadLevel()
    {
        await UniTask.Delay(500);

        if (SceneManager.GetSceneByName("Level").isLoaded)
        {
            await SceneManager.UnloadSceneAsync("Level");
            await UniTask.DelayFrame(5);
        }

        await SceneManager.LoadSceneAsync("Level", LoadSceneMode.Additive);
    }

    public void Initialize()
    {
        
    }

    public void StartGame()
    {
        LoadLevel().Forget();
    }
}