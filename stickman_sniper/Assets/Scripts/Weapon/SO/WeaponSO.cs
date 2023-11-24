using System;
using UnityEngine;

[Serializable]
public class WeaponModel
{
    public GameObject View;

    public string Key;
    public int BulletType;
    public float Damage;
    public float ReloadingTime;
    public int MaxBulletsCount;
    public int MagazineCapacity;
    public int TimeBetweenShots;
    public float PushForce;
}

public abstract class WeaponSO : ScriptableObject
{
    public abstract BaseWeapon GetWeapon();
    public WeaponModel Model;    
}
