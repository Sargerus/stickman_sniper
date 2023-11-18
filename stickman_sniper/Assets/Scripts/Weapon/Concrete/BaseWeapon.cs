using DWTools;
using UniRx;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour, IWeapon
{
    public int BulletType { get; protected set; }

    private ReactiveProperty<int> _currentBulletsCount = new();
    public IReadOnlyReactiveProperty<int> CurrentBulletsCount => _currentBulletsCount;

    public int MaxBulletsCount { get; protected set; }

    private ReactiveProperty<bool> _canShoot = new();
    public IReadOnlyReactiveProperty<bool> CanShoot => _canShoot;

    private ReactiveProperty<bool> _isGrabing = new();
    public IReadOnlyReactiveProperty<bool> IsGrabing => _isGrabing;

    private ReactiveProperty<bool> _isReloading = new();
    public IReadOnlyReactiveProperty<bool> IsReloading => _isReloading;

    public virtual void Grab()
    {
    }

    public virtual void Reload()
    {
        _currentBulletsCount.Value = 0;
    }

    public virtual void Shoot()
    {
        _currentBulletsCount.Value--;
    }
}
