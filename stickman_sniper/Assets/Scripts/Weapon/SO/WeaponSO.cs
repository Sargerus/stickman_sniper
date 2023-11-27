using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AudioContainer
{
    public string Name;
    public AudioClip AudioClip;
}

[Serializable]
public class WeaponModel
{
    public GameObject Prefab;

    public string Key;
    public int BulletType;
    public float Damage;
    public float ReloadingTime;
    public int MaxBulletsCount;
    public int MagazineCapacity;
    public int TimeBetweenShots;
    public float PushForce;

    public List<AudioContainer> AudioContainer;
}

public static class WeaponModelExtensions
{
    public static AudioClip GetAudioClip(this WeaponModel model, string name)
        => model.AudioContainer.FirstOrDefault(g => g.Name.Equals(name)).AudioClip;
}

public abstract class WeaponSO : ScriptableObject
{
    public abstract BaseWeapon GetWeapon();
    public WeaponModel Model;    
}
