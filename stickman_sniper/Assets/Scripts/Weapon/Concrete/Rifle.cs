using DWTools;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class Rifle : BaseWeapon
{
    [Inject(Id = CameraProvider.WorldCamera)]
    private CameraProvider _fpsCamera;

    [Inject]
    private IAudioManager _audioManager;

    private CompositeDisposable _disposables = new();
    private CompositeDisposable _soundsDisposables = new();

    public override void Shoot()
    {
        if (!CanShoot.Value)
            return;

        _isShooting.Value = true;
        _currentBulletsCount.Value--;
        Observable.Timer(TimeSpan.FromMilliseconds(TimeBetweenShots)).Subscribe(_ =>
        {
            if (_isShooting == null)
                return;

            _isShooting.Value = false;
        }).AddTo(_disposables);

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
            Disposable.CreateWithState(sourceReload, (s) => s.Stop()).AddTo(_soundsDisposables);
        }).AddTo(_disposables);

        var layerMask = 1 << LayerMask.NameToLayer("Target");
        Ray ray = _fpsCamera.Camera.ViewportPointToRay(new(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out var hit, 100f, layerMask))
        {   
            var enemy = hit.transform.GetComponentInParent<Enemy>();
            if (enemy == null)
                return;

            enemy.PrepareForDeath();
            Vector3 direction = (hit.point - _fpsCamera.transform.position).normalized;
            direction.y = 0.5f;
            hit.rigidbody.AddForce(direction * _model.PushForce, ForceMode.Impulse);
        }
    }

    public override void Aim()
    {
        _isAiming.Value = !_isAiming.Value;

        var aimSource = _audioManager.GetSource();
        aimSource.gameObject.SetActive(true);
        aimSource.Play(_model.GetAudioClip(AudioConstants.Aim));
        Disposable.CreateWithState(aimSource, (s) => s.Stop()).AddTo(_soundsDisposables);
    }

    public override void Dispose()
    {
        _soundsDisposables.Clear();
        _disposables.Clear();
    }
}
