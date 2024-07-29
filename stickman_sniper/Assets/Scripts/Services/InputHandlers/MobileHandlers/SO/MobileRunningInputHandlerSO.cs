using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/Running", fileName = "RunningInputHandlerSO")]
public class MobileRunningInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new RunningMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
