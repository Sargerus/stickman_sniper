using DW.RandomExtensions;
using UnityEngine;
using YG;
using Zenject;

public class LevelConstructor : IInitializable
{
    private readonly LevelsContainerSO _levelsContainer;
    private readonly ILevelLoader _levelLoader;
    private readonly DiContainer _diContainer;
    private readonly Transform _levelContainer;

    private FirstPersonController _player;
    private Level _currentLevel;

    public LevelConstructor(LevelsContainerSO levelsContainer, ILevelLoader levelLoader, DiContainer diContainer)
    {
        _levelsContainer = levelsContainer;
        _levelLoader = levelLoader;
        _diContainer = diContainer;
    }

    public void Initialize()
    {
        InstantiateLevel();
    }

    private void InstantiateLevel()
    {
        Level levelPrefab = null;

        if (_currentLevel != null)
        {
            GameObject.Destroy(_currentLevel.gameObject);
        }

        levelPrefab = _levelsContainer.Levels.CycleWithMod(YandexGame.savesData.levelsPassed);
        var levelGO = _diContainer.InstantiatePrefab(levelPrefab);
        _currentLevel = levelGO.GetComponent<Level>();
        _currentLevel.AimComplete += AimComplete;
    }

    private void AimComplete()
    {
        _currentLevel.AimComplete -= AimComplete;
        YandexGame.savesData.levelsPassed++;
        YandexGame.SaveProgress();

        _levelLoader.LoadLevel();
    }
}
