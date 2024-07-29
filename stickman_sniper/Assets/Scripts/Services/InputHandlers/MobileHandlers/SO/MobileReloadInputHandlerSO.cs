using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Mobile/Reloading", fileName = "ReloadingInputHandlerSO")]
public class MobileReloadInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler() 
    {
        var handler = new ReloadingMobileHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
