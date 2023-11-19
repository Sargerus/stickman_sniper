using DWTools;
using UniRx;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour, IWeapon, IWeaponStateController
{
    protected WeaponModel _model;

    protected ReactiveProperty<int> _currentBulletsCount;
    public IReadOnlyReactiveProperty<int> CurrentBulletsCount => _currentBulletsCount;

    public IReadOnlyReactiveProperty<bool> CanShoot { get; private set; }

    protected ReactiveProperty<bool> _isGrabing;
    public IReadOnlyReactiveProperty<bool> IsGrabing => _isGrabing;

    protected ReactiveProperty<bool> _isReloading;
    public IReadOnlyReactiveProperty<bool> IsReloading => _isReloading;

    protected ReactiveProperty<bool> _isShooting;
    public IReadOnlyReactiveProperty<bool> IsShooting => _isShooting;

    public string Key => _model.Key;
    public int BulletType => _model.BulletType;
    public float Damage => _model.Damage;
    public float ReloadingTime => _model.ReloadingTime;
    public int MaxBulletsCount => _model.MaxBulletsCount;
    public int MagazineCapacity => _model.MagazineCapacity;
    public int TimeBetweenShots => _model.TimeBetweenShots;

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

    public void SetIsGrabing(bool isGrabing)
        => _isGrabing.Value = isGrabing;

    public void SetIsReloading(bool isReloading)
        => _isReloading.Value = isReloading;

    public void SetIsShooting(bool isShooting)
        => _isShooting.Value = isShooting;

    public void Initialize(WeaponModel model)
    {
        _model = model;

        _isShooting = new();
        _isReloading = new();
        _isGrabing = new();
        _currentBulletsCount = new();

        CanShoot = Observable.CombineLatest<bool, bool, bool>(_isReloading, _isGrabing, (l, r) => !(l || r)).ToReactiveProperty();
    }
}
