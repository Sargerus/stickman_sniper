using DWTools;
using System;
using UniRx;
using Unity.VisualScripting;

[Serializable]
public class WeaponState
{
    public readonly int CurrentBulletsCount;
    public readonly int StashedBulletsCount;

    public WeaponState(int currentBulletsCount, int stashedBulletsCount)
    {
        CurrentBulletsCount = currentBulletsCount;
        StashedBulletsCount = stashedBulletsCount;
    }
}

public interface IWeaponService
{
    void SwitchToWeaponSlot(int slot);
    void SwitchToWeapon(string weaponKey);

    IReadOnlyReactiveProperty<IWeapon> CurrentWeapon { get; }
}

public sealed class WeaponService : IWeaponService, IDisposable
{
    private readonly WeaponFactory _weaponFactory;
    private readonly WeaponsContainerSO _weaponsContainer;

    private ReactiveProperty<IWeapon> _currentWeapon = new();
    public IReadOnlyReactiveProperty<IWeapon> CurrentWeapon => _currentWeapon;

    public WeaponService(WeaponFactory weaponFactory,
        WeaponsContainerSO weaponsContainer)
    {
        _weaponFactory = weaponFactory;
        _weaponsContainer = weaponsContainer;
    }

    public void SwitchToWeaponSlot(int slot)
    {
    }

    public void SwitchToWeapon(string weaponKey)
    {
        var weapon = _weaponFactory.Create(weaponKey);
        weapon.Initialize(FindModel(weaponKey), new(10, 0));

        _currentWeapon.Value?.Dispose();
        _currentWeapon.Value = weapon;
    }

    private WeaponModel FindModel(string key)
    {
        var weaponSO = _weaponsContainer.Get(key);
        return weaponSO.Model;
    }

    public void Dispose()
    {
        _currentWeapon.Value?.Dispose();
        _currentWeapon?.Dispose();
    }
}
