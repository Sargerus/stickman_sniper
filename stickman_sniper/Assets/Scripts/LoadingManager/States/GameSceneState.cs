using Cysharp.Threading.Tasks;
using DWTools;
using DWTools.Windows;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public sealed class GameSceneState : SceneState
{
    [Inject] private IUIManager _uiManager;

    private PreloadBehavior _preloadBehavior;
    private InjectBehaviorManagerTasks _injectBehaviorManagerTasks;

    public GameSceneState(AssetReference scene) : base(scene)
    {
    }

    public override async UniTask OnEnterState()
    {
        await base.OnEnterState();

        _injectBehaviorManagerTasks = GameObject.FindObjectOfType<InjectBehaviorManagerTasks>();

        if (_sceneContext.TryGetComponent<PreloadBehavior>(out _preloadBehavior))
        {
            await _preloadBehavior.Load();
        }

        _sceneContext.Run();
        _sceneContext.Container.Inject(this);
        await AwaitUImanagerInitialized();
    }

    private async UniTask AwaitUImanagerInitialized()
    {
        await UniTask.WaitUntil(() => _uiManager != null);
        await UniTask.WaitUntil(() => GameObject.FindObjectsOfType<UICameraProvider>().Length > 0);
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
        await _preloadBehavior.Clear();
        await base.OnExitState();
        _injectBehaviorManagerTasks.ClearTreeData();
        GlobalBlackboard.Blackboard.SetValue(BlackboardConstants.GameOverBool, false);
    }
}