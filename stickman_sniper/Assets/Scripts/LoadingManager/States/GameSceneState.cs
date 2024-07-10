using Cysharp.Threading.Tasks;
using DWTools.Windows;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public sealed class GameSceneState : SceneState
{
    [Inject] private IUIManager _uiManager;

    public GameSceneState(AssetReference scene) : base(scene)
    {
    }

    public override async UniTask OnEnterState()
    {
        await base.OnEnterState();
        _sceneContext.Container.Inject(this);

        await AwaitUImanagerInitialized();
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
        GlobalBlackboard.Blackboard.SetValue(BlackboardConstants.GameOverBool, false);
    }
}