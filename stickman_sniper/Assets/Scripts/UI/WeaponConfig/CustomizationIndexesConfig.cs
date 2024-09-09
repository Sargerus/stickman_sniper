using Customization;
using UnityEngine;

namespace UI.Weapon
{
    [CreateAssetMenu(menuName = "[CustomizationIndexesConfig]/CustomizationConfig", fileName = "new CustomizationConfig")]
    public class CustomizationIndexesConfig : ScriptableObject
    {
        public string Name;
        public WeaponCharacteristicsScriptable DefaultCustomizationData;
        public WeaponCharacteristicsScriptable CurrentCustomizationData;
    }
}