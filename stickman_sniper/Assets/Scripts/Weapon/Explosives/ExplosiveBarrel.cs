using stickman_sniper.Weapon.Explosives;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace stickman_sniper.Environment
{
    public class ExplosiveBarrel : MonoBehaviour, IExplosive
    {
        [SerializeField] private ExplosiveSettingsSO explosiveSettings;

        private Collider[] _buffer;

        private void Awake()
        {
            _buffer = new Collider[20];
        }

        public List<Collider> Explode()
        {
            Physics.OverlapSphereNonAlloc(transform.position, explosiveSettings.Radius, _buffer, explosiveSettings.LayerMask, QueryTriggerInteraction.Collide);
            int i = 0;
            while (i < _buffer.Length - 1 && _buffer[i] != null)
            {
                var rb = _buffer[i].GetComponent<MainRagdollRigidbody>();
                if (rb != null)
                {
                    var enemy = rb.GetComponentInParent<Enemy>();
                    enemy.PrepareForDeath();

                    Vector3 direction = (rb.transform.position - transform.position).normalized;
                    direction.y = explosiveSettings.UpwardModifier;
                    rb.Rigidbody.AddForce(direction * explosiveSettings.Force, ForceMode.Impulse);
                }

                _buffer[i] = null;
                i++;
            }

            Destroy(gameObject, 0.1f);
            return _buffer.ToList();
        }
    }
}