using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/XAxis", fileName = "XAxisInputHandlerSO")]
public class MobileXAxisInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new XAxisMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
