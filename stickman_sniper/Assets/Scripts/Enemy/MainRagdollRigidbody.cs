using StickmanSniper.Explosives;
using UnityEngine;

namespace StickmanSniper.Enemy
{

    public class MainRagdollRigidbody : MonoBehaviour, IExplodee
    {
        [SerializeField] private Rigidbody rb;

        public Rigidbody Rigidbody => rb;

        public bool IsExploded { get; private set; }

        public void Explode(ExplosiveSettingsSO explosionSettings)
        {
            IsExploded = true;

            if (TryGetComponent<MainRagdollRigidbody>(out var rb))
            {
                var enemy = rb.GetComponentInParent<Enemy>();
                if (!enemy.IsAlive.Value)
                    return;

                enemy.PrepareForDeath();

                Vector3 direction = (rb.transform.position - transform.position).normalized;
                direction.y = explosionSettings.UpwardModifier;
                rb.Rigidbody.AddForce(direction * explosionSettings.Force, ForceMode.Impulse);
            }
        }
    }
}