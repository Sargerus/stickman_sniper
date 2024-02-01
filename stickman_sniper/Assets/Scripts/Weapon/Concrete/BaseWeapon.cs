using DWTools;
using UniRx;
using UnityEngine;

public abstract class BaseWeapon : IWeapon
{
    protected WeaponModel _model;

    protected ReactiveProperty<int> _currentBulletsCount;
    public IReadOnlyReactiveProperty<int> CurrentBulletsCount => _currentBulletsCount;

    protected ReactiveProperty<int> _stashedBulletsCount;
    public IReadOnlyReactiveProperty<int> StashedBulletsCount => _stashedBulletsCount;

    public IReadOnlyReactiveProperty<bool> CanShoot { get; private set; }

    protected ReactiveProperty<bool> _isGrabing;
    public IReadOnlyReactiveProperty<bool> IsGrabing => _isGrabing;

    protected ReactiveProperty<bool> _isReloading;
    public IReadOnlyReactiveProperty<bool> IsReloading => _isReloading;

    protected ReactiveProperty<bool> _isShooting;
    public IReadOnlyReactiveProperty<bool> IsShooting => _isShooting;

    protected ReactiveProperty<bool> _isAiming;
    public IReadOnlyReactiveProperty<bool> IsAiming => _isAiming;

    public string Key => _model.Key;
    public int BulletType => _model.BulletType;
    public float Damage => _model.Damage;
    public float ReloadingTime => _model.ReloadingTime;
    public int MaxBulletsCount => _model.MaxBulletsCount;
    public int MagazineCapacity => _model.MagazineCapacity;
    public int TimeBetweenShots => _model.TimeBetweenShots;
    public SwaySettings SwaySettings => _model.SwaySettings;
    public GameObject Prefab => _model.Prefab;
    public GameObject View { get; set; }

    public virtual void Grab()
    {
    }

    public virtual void Reload()
    {
        _currentBulletsCount.Value = 0;
    }

    public virtual void Shoot()
    {

    }

    public virtual void SetAim(bool aim)
    {
    }

    public virtual void Initialize(WeaponModel model, WeaponState weaponState)
    {
        _model = model;

        _isShooting = new();
        _isReloading = new();
        _isGrabing = new();
        _isAiming = new();
        _currentBulletsCount = new(weaponState.CurrentBulletsCount);
        _stashedBulletsCount = new(weaponState.StashedBulletsCount);

        CanShoot = Observable.CombineLatest<int, bool, bool, bool, bool>(_currentBulletsCount, 
            _isReloading, _isGrabing, _isShooting, (o1, o2, o3, o4) => !(o1 <= 0 || o2 || o3 || o4)).ToReactiveProperty();
    }

    public virtual void Dispose()
    {
    }
}
