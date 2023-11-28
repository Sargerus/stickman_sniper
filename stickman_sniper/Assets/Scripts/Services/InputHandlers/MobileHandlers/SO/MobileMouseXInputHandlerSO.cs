using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/MouseX", fileName = "MouseXInputHandlerSO")]
public class MobileMouseXInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new MouseXMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
