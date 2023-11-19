using DWTools;
using UnityEngine;
using Zenject;

public class Rifle : BaseWeapon
{
    [Inject(Id = CameraProvider.WorldCamera)]
    private CameraProvider _fpsCamera;

    public override void Shoot()
    {
        base.Shoot();

        var layerMask = 1 << LayerMask.NameToLayer("Target");
        Ray ray = _fpsCamera.Camera.ViewportPointToRay(new(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out var hit, 100f, layerMask))
        {
            Debug.Log(hit);
        }
    }
}
