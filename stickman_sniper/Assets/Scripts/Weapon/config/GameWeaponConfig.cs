using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "[GameWeaponConfig]/WeaponConfig", fileName = "new WeaponConfig")]
public class GameWeaponConfig : ScriptableObject
{
    public List<WeaponConfig> WeaponsConfig;

    public WeaponConfig GetConfig(string key)
    {
        return WeaponsConfig.FirstOrDefault(h => h.Name.Equals(key));
    }
}

[Serializable]
public class WeaponConfig
{
    public string Name;
    public AssetReference Prefab;
}