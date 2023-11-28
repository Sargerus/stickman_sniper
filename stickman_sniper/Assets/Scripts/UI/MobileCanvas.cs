using DWTools;
using UnityEngine;

public class MobileCanvas : MonoBehaviour, IMobileInputProvider
{
    [SerializeField] private UniversalMobileController.FloatingJoyStick _moveJoyStick;
    [SerializeField] private UniversalMobileController.SpecialTouchPad _cameraJoyStick;

    public float GetMouseX()
        => _cameraJoyStick.GetHorizontalValue();

    public float GetMouseY()
        => _cameraJoyStick.GetVerticalValue();

    public float GetMoveX()
        => _moveJoyStick.GetHorizontalValue();

    public float GetMoveY()
        => _moveJoyStick.GetVerticalValue();


}
