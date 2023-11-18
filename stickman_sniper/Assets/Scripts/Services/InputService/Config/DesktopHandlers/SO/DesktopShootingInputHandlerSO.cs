using DWTools;
using UnityEngine;

[CreateAssetMenu(menuName = "[INPUT]Input/Desktop/Shooting", fileName = "ShootingInputHandlerSO")]
public class DesktopShootingInputHandlerSO : BaseInputHandlerSO
{
    public override IInputHandler GetHandler() => new ShootingDesktopHandler();
}
