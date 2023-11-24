using System;
using UnityEngine;

[CreateAssetMenu(menuName = "[WEAPON]Weapon/WeaponSO/Rifle", fileName = "new Rifle")]
public class SniperRifleSO : WeaponSO
{
    private BaseWeapon _cachedValue;

    public override BaseWeapon GetWeapon()
    {
        _cachedValue ??= new Rifle();
        return _cachedValue;
    }
}