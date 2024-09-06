using Analytics;
using DWTools.Extensions;
using UnityEngine;
using YG;
using Zenject;

public class LevelConstructor : IInitializable
{
    private readonly LevelsContainerSO _levelsContainer;
    private readonly DiContainer _diContainer;
    private readonly CurrentLevelService _currentLevelService;

    private Level _currentLevel;

    public LevelConstructor(LevelsContainerSO levelsContainer, DiContainer diContainer, CurrentLevelService currentLevelService)
    {
        _levelsContainer = levelsContainer;
        _diContainer = diContainer;
        _currentLevelService = currentLevelService;
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

        _currentLevelService.CurrentLevel = YandexGame.savesData.levelsPassed % _levelsContainer.Levels.Count;
        levelPrefab = _levelsContainer.Levels.CycleWithMod(YandexGame.savesData.levelsPassed);

        if (_levelsContainer.TestLevel != null)
        {
            levelPrefab = _levelsContainer.TestLevel;
        }

        var levelGO = _diContainer.InstantiatePrefab(levelPrefab);

        AnalyticsEventFactory.GetLevelLoadedEvent().AddLevelNumber(_currentLevelService.CurrentLevel).Send();
    }
}
