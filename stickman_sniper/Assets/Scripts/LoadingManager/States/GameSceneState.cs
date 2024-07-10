using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public sealed class GameSceneState : SceneState
{
    public GameSceneState(AssetReference scene) : base(scene)
    {
    }

    public override async UniTask OnEnterState()
    {
        await base.OnEnterState();
        _sceneContext.Container.Inject(this);
    }

    public override async UniTask OnExitState()
    {
        await base.OnExitState();
        GlobalBlackboard.Blackboard.SetValue(BlackboardConstants.GameOverBool, false);
    }
}