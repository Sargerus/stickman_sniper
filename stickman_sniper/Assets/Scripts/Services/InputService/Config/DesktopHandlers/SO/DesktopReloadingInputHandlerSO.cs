using DWTools;
using UnityEngine;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Reloading", fileName = "ReloadingInputHandlerSO")]
public class DesktopReloadingInputHandlerSO : BaseInputHandlerSO
{
    public override IInputHandler GetHandler() => new ReloadingDesktopHandler();
}
