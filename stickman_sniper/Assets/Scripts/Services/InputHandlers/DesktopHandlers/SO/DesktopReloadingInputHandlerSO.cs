using DWTools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Reloading", fileName = "ReloadingInputHandlerSO")]
public class DesktopReloadingInputHandlerSO : BaseInputHandlerSO
{
    [Inject] private DiContainer _diContainer;

    public override IInputHandler GetHandler()
    {
        var handler = new ReloadingDesktopHandler();
        _diContainer.Inject(handler);

        return handler;
    }
}
