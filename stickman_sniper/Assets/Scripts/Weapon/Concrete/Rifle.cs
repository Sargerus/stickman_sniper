using DWTools;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class Rifle : BaseWeapon
{
    [Inject(Id = CameraProvider.WorldCamera)]
    private CameraProvider _fpsCamera;

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
        });

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
}
