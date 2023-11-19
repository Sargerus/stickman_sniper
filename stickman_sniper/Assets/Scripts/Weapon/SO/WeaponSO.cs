using UnityEngine;

[CreateAssetMenu(menuName = "[WEAPON]Weapon/WeaponSO", fileName ="new Weapon")]
public class WeaponSO : ScriptableObject
{
    public string Key;
    public BaseWeapon Value;

    public float ReloadingTime;
    public float Damage;
    public int BulletType;
    public int MaxBulletsCount;
    public int MagazineCapacity;
    public int TimeBetweenShots;
}
