using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/ZAxis", fileName = "ZAxisInputHandlerSO")]
public class DesktopZAxisInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new ZAxisDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
