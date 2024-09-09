using TMPro;
using UnityEngine;
using YG;
using Zenject;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneAddressablesContainer _container;
    [SerializeField] private WeaponCharacteristicsContainer weaponCharacteristicsContainer;
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

        LoadingManager loadingManager = new(_container, weaponCharacteristicsContainer);
        loadingManager.StartGameMachine();
        //_completedLevels.SetText($" {YandexGame.savesData.levelsPassed + 1}/47");
    }

    //private async UniTaskVoid OpenCustomizeWindow()
    //{
    //    await UniTask.WaitUntil(() => _sceneContext.HasResolved);
    //
    //    var uiManager = _sceneContext.Container.Resolve<IUIManager>();
    //    uiManager.SetCamera(uiCamera);
    //    var handler = await uiManager.CreateWindow("customization_screen", new("customization_screen"));
    //    await handler.Show(false);
    //    await UniTask.WaitUntil(() => handler.CurrentState.Value == Window.LifecycleState.Hidden);
    //}
}
