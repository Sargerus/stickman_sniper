using DWTools;
using UnityEngine;

public interface IWeaponService
{
    void SwitchToWeaponSlot(int slot);
    void SwitchToWeapon(string weaponKey);
}

public class WeaponService : IWeaponService
{
    private readonly WeaponFactory _weaponFactory;
    private readonly IHandsController _handsController;
    private readonly WeaponsContainerSO _weaponsContainer;

    private IWeaponStateController _stateController;

    public WeaponService(WeaponFactory weaponFactory,
        IHandsController handsController,
        WeaponsContainerSO weaponsContainer)
    {
        _weaponFactory = weaponFactory;
        _handsController = handsController;
        _weaponsContainer = weaponsContainer;
    }

    public void SwitchToWeaponSlot(int slot)
    {
    }

    public void SwitchToWeapon(string weaponKey)
    {
        var newWeapon = _weaponFactory.Create(weaponKey, _handsController.WeaponContainer.transform.position,
            Quaternion.identity, _handsController.WeaponContainer.transform, out _stateController);

        newWeapon.Initialize(FindModel(weaponKey));

        _handsController.SwitchWeapon(newWeapon, _stateController);
    }

    private WeaponModel FindModel(string key)
    {
        var weaponSO = _weaponsContainer.Get(key);
        return new()
        {
            Key = weaponSO.Key,
            BulletType = weaponSO.BulletType,
            Damage = weaponSO.Damage,
            MagazineCapacity = weaponSO.MagazineCapacity,
            MaxBulletsCount = weaponSO.MaxBulletsCount,
            ReloadingTime = weaponSO.ReloadingTime
        };
    }
}
