using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/Aiming", fileName = "AimInputHandlerSO")]
public class MobileAimInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler() 
    {
        var handler = new AimingMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
