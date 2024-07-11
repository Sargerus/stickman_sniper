using Cysharp.Threading.Tasks;
using DW.StateMachine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;

public abstract class SceneState : IState
{
    protected SceneContext _sceneContext;
    private AssetReference _scene;
    protected AsyncOperationHandle<SceneInstance> _asyncOperationHandle;

    public SceneState(AssetReference scene)
    {
        _scene = scene;
    }

    public IGlobalBlackboard GlobalBlackboard { get; set; }

    public virtual async UniTask OnEnterState()
    {
        _asyncOperationHandle = _scene.LoadSceneAsync(LoadSceneMode.Additive, true);//true doesn't really work
        await _asyncOperationHandle.ToUniTask();

        await UniTask.DelayFrame(1); // wait additive scene to become active
        SceneManager.SetActiveScene(_asyncOperationHandle.Result.Scene);
        await UniTask.DelayFrame(1); // wait additive scene to become active

        var gameObjects = GameObject.FindGameObjectsWithTag(LoadingManager.SceneContextTag);
        GameObject sceneContextGameObject = null;

        foreach (var go in gameObjects)
        {
            if (go.scene.name.Equals(_asyncOperationHandle.Result.Scene.name))
            {
                sceneContextGameObject = go;
                break;
            }
        }

        if (sceneContextGameObject == null)
        {
            await OnExitState();
            return;
        }

        _sceneContext = sceneContextGameObject.GetComponent<SceneContext>();
        _sceneContext.Run();
    }

    public virtual async UniTask OnExitState()
    {
        await _scene.UnLoadScene();
    }

    public virtual void Tick(float elapsedTime)
    {
    }
}