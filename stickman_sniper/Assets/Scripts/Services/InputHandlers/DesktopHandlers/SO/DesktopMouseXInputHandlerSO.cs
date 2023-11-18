using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/MouseX", fileName = "MouseXInputHandlerSO")]
public class DesktopMouseXInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new MouseXDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
