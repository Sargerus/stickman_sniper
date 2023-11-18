using DWTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "[WEAPON]Weapon/WeaponsContainerSO", fileName = "new WeaponsContainer")]
public class WeaponsContainerSO : ScriptableObject
{
    public List<WeaponSO> Weapons;

    public IWeapon Get(string key)
    {
        return Weapons.FirstOrDefault(g => g.Key == key)?.Value;
    }
}
