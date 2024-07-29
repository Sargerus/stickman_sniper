using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/Jumping", fileName = "JumpingInputHandlerSO")]
public class MobileJumpInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new JumpingMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
