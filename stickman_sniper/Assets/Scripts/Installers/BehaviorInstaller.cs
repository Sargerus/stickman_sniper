using UnityEngine;
using Zenject;

public class BehaviorInstaller : MonoInstaller
{
    [SerializeField] private InjectBehaviorManagerTasks injectBehaviorManagerTasks;

    public override void InstallBindings()
    {
        Container.BindInstance(injectBehaviorManagerTasks).AsSingle();
    }
}