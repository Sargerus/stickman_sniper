using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "BootstrapSOInstaller", fileName = "BootstrapScriptableObjectInstaller")]
public class BootstrapScriptableObjectInstaller : ScriptableObjectInstaller
{
    [SerializeField] public InputHandlerContainerAggregatorSO _inputAggregator;
    [SerializeField] public LevelsContainerSO _levelsContainer;

    public override void InstallBindings()
    {
        Container.BindInstances(_inputAggregator, _levelsContainer);

        _inputAggregator.InputHandlersContainer.ForEach(container =>
        {
            container.InputHandlers.ForEach(handler => Container.QueueForInject(handler));
        });
    }
}