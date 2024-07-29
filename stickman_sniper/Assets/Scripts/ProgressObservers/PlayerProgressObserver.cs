using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using InfimaGames.LowPolyShooterPack;
using System;
using UniRx;
using Zenject;

public interface IPlayerProgressObserver : IProgressObserver
{

}

public class PlayerProgressObserver : IPlayerProgressObserver, IInitializable, IDisposable
{
    private readonly ILevelProgressObserver _levelProgressObserver;
    //private readonly IWeaponService _weaponService;
    private readonly Character _character;

    private IDisposable _bulletsDisposable;
    private CompositeDisposable _disposables = new();

    private ReactiveProperty<bool> _lose = new(false);
    public IReadOnlyReactiveProperty<bool> Lose => _lose;

    private ReactiveProperty<bool> _win = new(false);
    public IReadOnlyReactiveProperty<bool> Win => _win;

    public PlayerProgressObserver(ILevelProgressObserver levelProgressObserver, Character character)//, IWeaponService weaponService)
    {
        _levelProgressObserver = levelProgressObserver;
        _character = character;
    }

    public void Initialize()
    {
        Observable.EveryUpdate().Subscribe(_ => ObserveAmmunitionCount()).AddTo(_disposables);
    }

    private void ObserveAmmunitionCount()
    {
        if (!_character.IsInitialized)
            return;

        var equippedWeapon = _character.GetInventory().GetEquipped();
        int currentAmmo = equippedWeapon.GetAmmunitionCurrent() + equippedWeapon.GetAmmunitionSpareLeft();
        if (currentAmmo <= 0 && _levelProgressObserver.KilledEnemies.Value < _levelProgressObserver.TotalEnemies)
            _lose.Value = true;
    }

    public void Dispose()
    {
        _bulletsDisposable?.Dispose();
        _disposables.Dispose();
    }
}