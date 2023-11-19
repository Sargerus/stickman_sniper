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

        var layerMask = 1 << LayerMask.NameToLayer("Target");
        Ray ray = _fpsCamera.Camera.ViewportPointToRay(new(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out var hit, 100f, layerMask))
        {
            _currentBulletsCount.Value--;
            Debug.Log(hit);

            _isShooting.Value = true;
            Observable.Timer(TimeSpan.FromMilliseconds(TimeBetweenShots)).Subscribe(_ =>
            {
                if (_isShooting == null)
                    return;

                _isShooting.Value = false;
            });
        }
    }
}
