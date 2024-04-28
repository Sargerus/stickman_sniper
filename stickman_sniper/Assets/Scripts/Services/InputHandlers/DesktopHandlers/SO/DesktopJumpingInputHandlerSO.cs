using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Jumping", fileName = "JumpingInputHandlerSO")]
public class DesktopJumpingInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new JumpingDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
