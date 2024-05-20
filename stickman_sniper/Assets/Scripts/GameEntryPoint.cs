using Cysharp.Threading.Tasks;
using DWTools.Windows;
using TMPro;
using UnityEngine;
using YG;
using Zenject;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneContext _sceneContext;
    [SerializeField] private Camera uiCamera;
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
        if (_isInitialize)
            return;

        _isInitialize = true;
        _completedLevels.SetText($" {YandexGame.savesData.levelsPassed + 1}/47");

        _sceneContext.Run();

        OpenCustomizeWindow().Forget();
    }

    private async UniTaskVoid OpenCustomizeWindow()
    {
        await UniTask.WaitUntil(() => _sceneContext.HasResolved);

        var uiManager = _sceneContext.Container.Resolve<IUIManager>();
        uiManager.SetCamera(uiCamera);
        var handler = await uiManager.CreateWindow("customization_screen", new("customization_screen"));
        await handler.Show(false);
        await UniTask.WaitUntil(() => handler.CurrentState.Value == Window.LifecycleState.Hidden);
    }
}
