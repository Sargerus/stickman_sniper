using UnityEngine;

namespace StickmanSniper.Explosives
{
    [CreateAssetMenu(menuName = "[WEAPON]Weapon/Explosive/Settings", fileName = "new ExplosiveSettings")]
    public class ExplosiveSettingsSO : ScriptableObject
    {
        public LayerMask LayerMask;
        public float Radius;
        public float Force;
        public float UpwardModifier;
        public GameObject ExplosionEffect;
    }
}