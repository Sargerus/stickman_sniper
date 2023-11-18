using UnityEngine.SceneManagement;
using Zenject;

public class LevelLoader : IInitializable
{
    public void SwitchLevel()
    {
        SceneManager.LoadSceneAsync("Level", LoadSceneMode.Additive);
    }

    public void Initialize()
    {
        SwitchLevel();
    }
}