using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Aiming", fileName = "AimInputHandlerSO")]
public class DesktopAimInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler() 
    {
        var handler = new AimingDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
