using UnityEngine;

[CreateAssetMenu(menuName = "[WEAPON]Weapon/WeaponSO", fileName ="new Weapon")]
public class WeaponSO : ScriptableObject
{
    public string Key;
    public BaseWeapon Value;
}
