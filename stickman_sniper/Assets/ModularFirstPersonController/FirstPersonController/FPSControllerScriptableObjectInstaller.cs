using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "FPSControllerSOInstaller", fileName = "FPSControllerScriptableObjectInstaller")]
public class FPSControllerScriptableObjectInstaller : ScriptableObjectInstaller
{
    [SerializeField] private InputHandlerContainerAggregatorSO _inputAggregator;

    public override void InstallBindings()
    {
        Container.BindInstances(_inputAggregator);

        _inputAggregator.InputHandlersContainer.ForEach(container =>
        {
            container.InputHandlers.ForEach(handler => Container.QueueForInject(handler));
        });
    }
}
