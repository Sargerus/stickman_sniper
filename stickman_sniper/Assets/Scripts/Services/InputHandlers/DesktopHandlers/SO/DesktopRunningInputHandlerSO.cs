using DWTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Running", fileName = "RunningInputHandlerSO")]
public class DesktopRunningInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new RunningDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
