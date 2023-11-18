using DWTools;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "[INPUT]Input/InputHandlerContainer", fileName = "new InputHandlerContainer")]
public class InputHandlerContainerSO : ScriptableObject
{
    public Device Device;
    public List<BaseInputHandlerSO> InputHandlers;
}
