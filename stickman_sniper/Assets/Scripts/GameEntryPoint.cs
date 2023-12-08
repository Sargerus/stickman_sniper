using TMPro;
using UnityEngine;
using YG;
using Zenject;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneContext _sceneContext;
    [SerializeField] private TMP_Text _completedLevels;

    private bool _isInitialize = false;

    private void OnEnable() => YandexGame.GetDataEvent += OnYGInitialized;
    private void OnDisable() => YandexGame.GetDataEvent -= OnYGInitialized;

    private void Start()
    {
        if (!YandexGame.SDKEnabled)
            return;

        OnYGInitialized();   
    }

    private void OnYGInitialized()
    {
        if(_isInitialize) 
            return;

        _isInitialize = true;
        _completedLevels.SetText($" {YandexGame.savesData.levelsPassed + 1}/47");

        _sceneContext.Run();
    }
}
