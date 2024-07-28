using DWTools;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
using UniRx.Triggers;
using System.Linq;

public class MobileCanvas : MonoBehaviour, IMobileInputProvider
{
    [SerializeField] private UniversalMobileController.FloatingJoyStick _moveJoyStick;
    [SerializeField] private UniversalMobileController.SpecialTouchPad _cameraJoyStick;
    [SerializeField] private List<Button> _fireButton;

    private bool _isAiming = false;
    private bool _isFiring = false;
    private CompositeDisposable _fireDisposables = new();
    private int? _cachedShootTouch = null;

    private void Start()
    {
        Observable.Merge(_fireButton.Select(g => g.OnPointerDownAsObservable())).Subscribe(data =>
        {
            _cachedShootTouch = data.pointerId;
            _isFiring = true;
        }).AddTo(_fireDisposables);

        Observable.Merge(_fireButton.Select(g => g.OnPointerUpAsObservable())).Subscribe(_ =>
        {
            _isFiring = false;
        }).AddTo(_fireDisposables);
    }

    private void LateUpdate()
    {
        if (_cachedShootTouch == null || Input.touchCount == 0
            || !Input.touches.Any(g => g.fingerId == _cachedShootTouch))
        {
            _isFiring = false;
            return;
        }
    }

    public float GetMouseX()
        => _cameraJoyStick.GetHorizontalValue();

    public float GetMouseY()
        => _cameraJoyStick.GetVerticalValue();

    public float GetMoveX()
        => _moveJoyStick.GetHorizontalValue();

    public float GetMoveY()
        => _moveJoyStick.GetVerticalValue();

    public bool IsAiming()
        => _isAiming;

    public void SetIsAiming()
        => _isAiming = !_isAiming;

    public bool IsShooting()
        => _isFiring;

    private void OnDestroy()
    {
        _fireDisposables.Dispose();
    }
}
