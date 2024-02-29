using UnityEngine;

namespace stickman_sniper.Weapon.Explosives
{
    [CreateAssetMenu(menuName = "[WEAPON]Weapon/Explosive/Settings", fileName = "new ExplosiveSettings")]
    public class ExplosiveSettingsSO : ScriptableObject
    {
        public LayerMask LayerMask;
        public float Radius;
        public float Force;
        public float UpwardModifier;
    }
}