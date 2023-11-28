using System;
using UniRx;
using Zenject;

public interface IPlayerProgressObserver : IProgressObserver
{

}

public class PlayerProgressObserver : IPlayerProgressObserver, IInitializable, IDisposable
{
    private readonly ILevelProgressObserver _levelProgressObserver;
    private readonly IWeaponService _weaponService;

    private IDisposable _bulletsDisposable;
    private CompositeDisposable _disposables = new();

    private ReactiveProperty<bool> _lose = new(false);
    public IReadOnlyReactiveProperty<bool> Lose => _lose;

    private ReactiveProperty<bool> _win = new(false);
    public IReadOnlyReactiveProperty<bool> Win => _win;

    public PlayerProgressObserver(ILevelProgressObserver levelProgressObserver, IWeaponService weaponService)
    {
        _levelProgressObserver = levelProgressObserver;
        _weaponService = weaponService;
    }

    public void Initialize()
    {
        _weaponService.CurrentWeapon.Subscribe(weapon =>
        {
            if (weapon == null)
                return;

            _bulletsDisposable?.Dispose();

            _bulletsDisposable = weapon.CurrentBulletsCount.Subscribe(bullets =>
            {
                if (bullets == 0 && _levelProgressObserver.KilledEnemies.Value < _levelProgressObserver.TotalEnemies)
                    _lose.Value = true;
            });
        }).AddTo(_disposables);
    }

    public void Dispose()
    {
        _bulletsDisposable?.Dispose();
        _disposables.Dispose();
    }
}