using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Crouching", fileName = "CrouchInputHandlerSO")]
public class DesktopCrouchingInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new CrouchingDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
