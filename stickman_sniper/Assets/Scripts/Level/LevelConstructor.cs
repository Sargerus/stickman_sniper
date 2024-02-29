using DWTools.Extensions;
using UnityEngine;
using YG;
using Zenject;

public class LevelConstructor : IInitializable
{
    private readonly LevelsContainerSO _levelsContainer;
    private readonly DiContainer _diContainer;

    private Level _currentLevel;

    public LevelConstructor(LevelsContainerSO levelsContainer, DiContainer diContainer)
    {
        _levelsContainer = levelsContainer;
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

        if (_levelsContainer.TestLevel != null)
        {
            levelPrefab = _levelsContainer.TestLevel;
        }

        var levelGO = _diContainer.InstantiatePrefab(levelPrefab);
    }
}
