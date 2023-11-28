using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/MouseY", fileName = "MouseYInputHandlerSO")]
public class MobileMouseYInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new MouseYMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
