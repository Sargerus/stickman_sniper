using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/ZAxis", fileName = "ZAxisInputHandlerSO")]
public class MobileZAxisInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new ZAxisMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
