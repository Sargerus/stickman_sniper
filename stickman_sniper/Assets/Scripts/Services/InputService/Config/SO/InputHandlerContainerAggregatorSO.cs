using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "[INPUT]Input/InputHandlerContainerAggreagator", fileName = "new InputHandlerContainerAggreagator")]
public class InputHandlerContainerAggregatorSO : ScriptableObject
{
    public List<InputHandlerContainerSO> InputHandlersContainer;
}
