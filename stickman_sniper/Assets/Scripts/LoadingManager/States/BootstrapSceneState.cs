using Currency;
using Cysharp.Threading.Tasks;
using DW.StateMachine;
using UnityEngine;
using Zenject;

public sealed class BootstrapSceneState : IState
{
    public IGlobalBlackboard GlobalBlackboard { get; set; }
    private LoadingManager _loadingManager;
    private SceneContext _sceneContext;

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

        _sceneContext = gameObject.GetComponent<SceneContext>();
        _sceneContext.Run();

        var loadingManagerHolder = _sceneContext.Container.Resolve<ILoadingManagerHolder>();
        loadingManagerHolder.LoadingManager = _loadingManager;

        InitializeCurrency();

        GlobalBlackboard.Blackboard.SetValue<bool>(BlackboardConstants.BootstrapReadyBool, true);
    }

    private void InitializeCurrency()
    {
        var currencyService = _sceneContext.Container.Resolve<ICurrencyService>();

        currencyService.CreateCurrency("gold");
    }

    public async UniTask OnExitState()
    {
    }

    public void Tick(float elapsedTime)
    {
    }
}