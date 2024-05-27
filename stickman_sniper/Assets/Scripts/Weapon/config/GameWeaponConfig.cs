using DWTools.Customization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "[GameWeaponConfig]/WeaponConfig", fileName = "new WeaponConfig")]
public class GameWeaponConfig : ScriptableObject
{
    public List<WeaponContainer> WeaponsConfig;
}

[Serializable]
public class WeaponContainer
{
    public string Tag;
    public List<WeaponConfig> Weapons;
}

[Serializable]
public class WeaponConfig
{
    public string Name;
    public AssetReference Prefab;
    public WeaponCharacteristicsScriptable DefaultCustomizationData;
    public WeaponCharacteristicsScriptable CurrentCustomizationData;
    //public List<CustomizeItem> CustomizationData;
}