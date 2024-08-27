using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/Crouching", fileName = "CrouchingInputHandlerSO")]
public class MobileCrouchingInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new CrouchingMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
