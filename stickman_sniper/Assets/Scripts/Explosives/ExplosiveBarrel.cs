using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StickmanSniper.Explosives
{
    public interface IExplodee
    {
        bool IsExploded { get; }
        void Explode(ExplosiveSettingsSO explosionSettings);
    }

    public class ExplosiveBarrel : MonoBehaviour, IExplosive, IExplodee
    {
        [SerializeField] private ExplosiveSettingsSO explosiveSettings;

        public bool IsExploded { get; private set; }
        private Collider[] _buffer;

        private void Awake()
        {
            _buffer = new Collider[20];
        }

        //TODO COMMON INTERFACE INSTEAD OF SWITCH
        public List<Collider> Explode()
        {
            IsExploded = true;

            Physics.OverlapSphereNonAlloc(transform.position, explosiveSettings.Radius, _buffer, explosiveSettings.LayerMask, QueryTriggerInteraction.Collide);
            int i = 0;
            while (i < _buffer.Length - 1 && _buffer[i] != null)
            {
                if (_buffer[i].TryGetComponent<IExplodee>(out var explodee))
                {
                    if (!explodee.IsExploded)
                        explodee.Explode(explosiveSettings);
                }

                _buffer[i] = null;
                i++;
            }

            Instantiate(explosiveSettings.ExplosionEffect, transform.position, explosiveSettings.ExplosionEffect.transform.rotation, null);

            Destroy(gameObject, 0.1f);
            return _buffer.ToList();
        }

        public void Explode(ExplosiveSettingsSO explosionSettings)
        {
            if (IsExploded)
                return;

            Explode();
        }
    }
}