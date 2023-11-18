using DWTools;
using UnityEngine;

public interface IWeaponService
{
    void SwitchToWeaponSlot(int slot);
    void SwitchToWeapon(string weaponKey);

    IWeapon CurrentWeapon { get; }
}

public class WeaponService : IWeaponService
{
    private readonly WeaponFactory _weaponFactory;
    private readonly IHandsController _handsController;

    public IWeapon CurrentWeapon { get; private set; }


    public WeaponService(WeaponFactory weaponFactory, IHandsController handsController)
    {
        _weaponFactory = weaponFactory;
        _handsController = handsController;


    }

    public void SwitchToWeaponSlot(int slot)
    {
    }

    public void SwitchToWeapon(string weaponKey)
    {
        CurrentWeapon = _weaponFactory.Create(weaponKey, _handsController.WeaponContainer.transform.position,
            Quaternion.identity, _handsController.WeaponContainer.transform);

        _handsController.SwitchWeapon(CurrentWeapon);
    }
}
