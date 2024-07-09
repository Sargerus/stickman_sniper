using Cysharp.Threading.Tasks;
using DW.StateMachine;
using UnityEngine;
using Zenject;

public sealed class BootstrapSceneState : IState
{
    public IGlobalBlackboard GlobalBlackboard { get; set; }
    private LoadingManager _loadingManager;

    public BootstrapSceneState(LoadingManager loadingManager)
    {
        _loadingManager = loadingManager;        
    }

    public async UniTask OnEnterState()
    {
        var gameObject = GameObject.FindWithTag(LoadingManager.SceneContextTag);
        if (gameObject == null)
        {
            await OnExitState();
            return;
        }

        SceneContext sceneContext = gameObject.GetComponent<SceneContext>();
        sceneContext.Run();

        var loadingManagerHolder = sceneContext.Container.Resolve<ILoadingManagerHolder>();
        loadingManagerHolder.LoadingManager = _loadingManager;

        GlobalBlackboard.Blackboard.SetValue<bool>(BlackboardConstants.BootstrapReadyBool, true);
    }

    public async UniTask OnExitState()
    {
    }

    public void Tick(float elapsedTime)
    {
    }
}