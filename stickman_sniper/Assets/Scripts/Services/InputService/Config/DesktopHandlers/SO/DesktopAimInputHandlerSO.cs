using DWTools;
using UnityEngine;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Aiming", fileName = "AimInputHandlerSO")]
public class DesktopAimInputHandlerSO : BaseInputHandlerSO
{
    public override IInputHandler GetHandler() => new AimingDesktopHandler();
}
