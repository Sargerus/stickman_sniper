using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/Shooting", fileName = "ShootingInputHandlerSO")]
public class MobileShootingInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new ShootingMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
