using DWTools.Customization;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "FPSControllerSOInstaller", fileName = "FPSControllerScriptableObjectInstaller")]
public class FPSControllerScriptableObjectInstaller : ScriptableObjectInstaller
{
    [SerializeField] private WeaponsContainerSO _weaponContainer;
    [SerializeField] private InputHandlerContainerAggregatorSO _inputAggregator;
    [SerializeField] private CustomiationDataContainerSO customiationDataContainerSO;

    public override void InstallBindings()
    {
        Container.BindInstances(_weaponContainer, _inputAggregator, customiationDataContainerSO);

        _inputAggregator.InputHandlersContainer.ForEach(container =>
        {
            container.InputHandlers.ForEach(handler => Container.QueueForInject(handler));
        });
    }
}
