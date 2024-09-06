using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Customization
{
    [CreateAssetMenu(menuName = "[CUSTOMIZATION]Customization/Weapon characteristics container", fileName = "new Characteristics container")]
    public class WeaponCharacteristicsContainer : ScriptableObject
    {
        public List<WeaponCharacteristicsContainerItem> Config;
    }

    [Serializable]
    public class WeaponCharacteristicsContainerItem
    {
        public string WeaponKey;
        public WeaponCharacteristicsScriptable DefaultCustomizationData;
        public WeaponCharacteristicsScriptable CurrentCustomizationData;
    }
}