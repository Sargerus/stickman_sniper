using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public interface IPreloadBehavior
{
    UniTask Load();
    UniTask Clear();
}

public class PreloadBehavior : MonoBehaviour, IPreloadBehavior
{
    [SerializeField] private List<MonoBehaviour> preloadBehaviors;

    public async UniTask Clear()
    {
        foreach (var pb in preloadBehaviors)
        {
            await ((IPreloadBehavior)pb).Clear();
        }
    }

    public async UniTask Load()
    {
        foreach (var pb in preloadBehaviors)
        {
            await ((IPreloadBehavior)pb).Load();
        }
    }
}
