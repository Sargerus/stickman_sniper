using Cysharp.Threading.Tasks;
using DWTools.Windows;
using UnityEngine;
using UnityEngine.AddressableAssets;
using YG;
using Zenject;

public sealed class MainMenuSceneState : SceneState
{
    [Inject] private IUIManager _uiManager;

    private IWindowHandler _handler;

    public MainMenuSceneState(AssetReference scene) : base(scene)
    {
    }

    public override async UniTask OnEnterState()
    {
        await base.OnEnterState();

        _sceneContext.Run();
        _sceneContext.Container.Inject(this);

        await AwaitUImanagerInitialized();

        _handler = await _uiManager.CreateWindow("customization_screen", null, _sceneContext.Container);
        await _handler.Show(false);

        YandexGame.Instance._FullscreenShow();
    }

    private async UniTask AwaitUImanagerInitialized()
    {
        await UniTask.WaitUntil(() => _uiManager != null);
        
        var cameras = GameObject.FindObjectsOfType<UICameraProvider>();
        UICameraProvider uiCamera = null;
        foreach (var c in cameras)
        {
            if (c.gameObject.scene.name.Equals(_asyncOperationHandle.Result.Scene.name))
            {
                uiCamera = c;
                break;
            }
        }
        
        _uiManager.SetCamera(uiCamera.Camera);
    }

    public override async UniTask OnExitState()
    {
        await base.OnExitState();
        _handler.Close(true);
        GlobalBlackboard.Blackboard.SetValue(BlackboardConstants.MainMenuReadyBool, false);
    }
}