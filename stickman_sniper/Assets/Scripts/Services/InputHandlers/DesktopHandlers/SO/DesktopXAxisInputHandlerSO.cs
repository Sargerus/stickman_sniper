using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/XAxis", fileName = "XAxisInputHandlerSO")]
public class DesktopXAxisInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new XAxisDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
