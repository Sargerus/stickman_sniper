using Cysharp.Threading.Tasks;
using DWTools.Windows;
using UnityEngine.AddressableAssets;
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

        _sceneContext.Container.Inject(this);

        await UniTask.WaitUntil(() => _uiManager != null);

        _handler = await _uiManager.CreateWindow("customization_screen", null, _sceneContext.Container);
        await _handler.Show(false);
    }

    public override async UniTask OnExitState()
    {
        await base.OnExitState();
        GlobalBlackboard.Blackboard.SetValue(BlackboardConstants.MainMenuReadyBool, false);
    }
}