using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Shooting", fileName = "ShootingInputHandlerSO")]
public class DesktopShootingInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new ShootingDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
