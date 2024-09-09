using System.Collections.Generic;
using UnityEngine;

namespace UI.Weapon
{

    [CreateAssetMenu(menuName = "[GameWeaponConfig]/AvailableWeaponConfig", fileName = "new AvailableWeaponConfig")]
    public class AvailableWeaponConfig : ScriptableObject
    {
        public List<string> AvailableWeapons;
    }
}