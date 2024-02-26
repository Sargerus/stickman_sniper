using DWTools;
using DWTools.Slowmotion;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class Rifle : BaseWeapon
{
    [Inject(Id = CameraProvider.WorldCamera)]
    private CameraProvider _fpsCamera;
    [Inject(Id = "sniper")]
    private CameraProvider _sniperCamera;
    [Inject(Id = "weapon")]
    private CameraProvider _weaponCamera;

    [Inject] private IAudioManager _audioManager;
    [Inject] private IInputService _inputService;
    [Inject] private IBulletSlowmotionService _bulletSlowmotionService;

    private CompositeDisposable _disposables = new();
    private CompositeDisposable _soundsDisposables = new();

    private float _rememberFov;
    private float _rememberSensitivity;
    private bool _lockAim = false;
    private IDisposable _aimLock;

    private ViewReferencesProvider _viewReferencesProvider;

    public override async void Shoot()
    {
        if (!CanShoot.Value)
            return;

        //animation
        SetAim(false);
        LockAim();
        var animator = View.GetComponent<Animator>();
        animator.SetTrigger("Shot");

        //bullet
        if (_viewReferencesProvider == null)
        {
            _viewReferencesProvider = View.GetComponent<ViewReferencesProvider>();
        }
        var bullet = _viewReferencesProvider.BulletPool.Get();
        bullet.Item.gameObject.SetActive(true);
        bullet.Item.Push();

        //sound
        _soundsDisposables.Clear();
        var sourceShot = _audioManager.GetSource();
        var sourceReload = _audioManager.GetSource();

        sourceShot.gameObject.transform.position = View.transform.position;
        sourceShot.gameObject.SetActive(true);
        sourceShot.Play(_model.GetAudioClip(AudioConstants.Shot));
        Disposable.CreateWithState(sourceShot, (s) => s.Stop()).AddTo(_soundsDisposables);

        Observable.Timer(TimeSpan.FromMilliseconds(800f)).SubscribeWithState(sourceReload, (_, sourceReload) =>
        {
            sourceReload.gameObject.transform.position = View.transform.position;
            sourceReload.gameObject.SetActive(true);
            sourceReload.Play(_model.GetAudioClip(AudioConstants.Reload));
            Disposable.CreateWithState(sourceReload, (s) => s?.Stop()).AddTo(_soundsDisposables);
        }).AddTo(_disposables);

        //timer
        _isShooting.Value = true;
        _currentBulletsCount.Value--;
        Observable.Timer(TimeSpan.FromMilliseconds(TimeBetweenShots)).Subscribe(_ =>
        {
            if (_isShooting == null)
                return;

            _isShooting.Value = false;
            UnlockAim();
        }).AddTo(_disposables);

        //raycast
        var layerMask = 1 << LayerMask.NameToLayer("Target");
        Ray ray = _fpsCamera.Camera.ViewportPointToRay(new(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out var hit, 100f, layerMask))
        {
            var enemy = hit.transform.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                var bulletSlowmotion = GameObject.Instantiate(_model.SlowmotionPrefab);
                var bulletProducer = bulletSlowmotion.GetComponent<IBulletProducer>();
                var task = _bulletSlowmotionService.SendBulletInSlowmotionAsync(View.transform.position, hit.point, bulletProducer);
                await task;

                bulletSlowmotion.gameObject.SetActive(false);
                GameObject.Destroy(bulletSlowmotion, 0.1f);

                enemy.PrepareForDeath();
                Vector3 direction = (hit.point - _fpsCamera.transform.position).normalized;
                direction.y = 0.5f;
                hit.rigidbody.AddForce(direction * _model.PushForce, ForceMode.Impulse);
            }
        }
    }

    private void LockAim()
    {
        _lockAim = true;
        _aimLock = _inputService.DisableKey(Keys.Aiming);
    }
    private void UnlockAim()
    {
        _lockAim = false;
        _aimLock?.Dispose();
    }

    public override void SetAim(bool aim)
    {
        if (aim == _isAiming.Value || _lockAim)
            return;

        _isAiming.Value = aim;

        //sound
        var aimSource = _audioManager.GetSource();
        aimSource.gameObject.SetActive(true);
        aimSource.Play(_model.GetAudioClip(AudioConstants.Aim));
        Disposable.CreateWithState(aimSource, (s) => s.Stop()).AddTo(_soundsDisposables);

        //animation
        if (_isAiming.Value)
        {
            _sniperCamera.gameObject.SetActive(true);
            _weaponCamera.gameObject.SetActive(false);

            _rememberFov = _fpsCamera.Camera.fieldOfView;
            _rememberSensitivity = _inputService.MouseSensitivity;

            _fpsCamera.Camera.fieldOfView = _model.FOVOnAim;
            _inputService.MouseSensitivity = _model.SensitivityOnAim;
        }
        else
        {
            _sniperCamera.gameObject.SetActive(false);
            _weaponCamera.gameObject.SetActive(true);

            _fpsCamera.Camera.fieldOfView = _rememberFov;
            _inputService.MouseSensitivity = _rememberSensitivity;
        }
    }

    public override void Dispose()
    {
        _soundsDisposables.Clear();
        _disposables.Clear();
    }
}
