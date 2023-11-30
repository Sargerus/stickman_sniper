using DWTools;
using System.Linq;
using UnityEngine;
using UniversalMobileController;
using YG;
using Zenject;

public interface IFinger
{
    void SetFingerId(int? fingerId);
}

public class TouchController : MonoBehaviour
{
    [SerializeField] private RectTransform _leftTR;
    [SerializeField] private RectTransform _rightTR;

    [SerializeField] private FloatingJoyStick _left;
    [SerializeField] private SpecialTouchPad _right;

    private CameraProvider _mobileCameraProvider;

    private int? _leftFinger;
    private int? _rightFinger;
    private bool _leftSet;
    private bool _rightSet;

    private bool _isInitialized = false;
    private int _previousFrameTouchesCount;

    [Inject]
    private void Construct([Inject(Id = "mobile")] CameraProvider mobileCameraProvider)
    {
        _mobileCameraProvider = mobileCameraProvider;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized || YandexGame.Device.ToDevice() != Device.Mobile)
            return;

        if (Input.touches.Length == 0)
        {
            _leftFinger = null;
            _rightFinger = null;
            _left.SetFingerId(_leftFinger);
            _right.SetFingerId(_rightFinger);
            return;
        }

        if (_rightFinger is null)
        {
            foreach (var touch in Input.touches)
            {
                if (_leftFinger != null && _leftFinger == touch.fingerId)
                    continue;

                if (RectTransformUtility.RectangleContainsScreenPoint(_rightTR, touch.position, _mobileCameraProvider.Camera))
                {
                    _rightFinger = touch.fingerId;
                    _right.SetFingerId(_rightFinger);
                    break;
                }
            }
        }
        else
        {
            if (!Input.touches.Any(g => g.fingerId == _rightFinger))
            {
                _rightFinger = null;
                _right.SetFingerId(_rightFinger);
            }
        }

        if (_leftFinger is null)
        {
            foreach (var touch in Input.touches)
            {
                if (_rightFinger != null && _rightFinger == touch.fingerId)
                    continue;

                if (RectTransformUtility.RectangleContainsScreenPoint(_leftTR, touch.position, _mobileCameraProvider.Camera))
                {
                    _leftFinger = touch.fingerId;
                    _left.SetFingerId(_leftFinger);
                    break;
                }
            }
        }
        else
        {
            if (!Input.touches.Any(g => g.fingerId == _leftFinger))
            {
                _leftFinger = null;
                _left.SetFingerId(_leftFinger);
            }
        }
    }
}
