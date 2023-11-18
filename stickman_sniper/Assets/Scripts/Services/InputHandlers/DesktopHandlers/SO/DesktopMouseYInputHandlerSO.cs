using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/MouseY", fileName = "MouseYInputHandlerSO")]
public class DesktopMouseYInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new MouseYDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
