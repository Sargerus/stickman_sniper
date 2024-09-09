using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableWeapon : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void DisableRB()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            _rigidbody.isKinematic = true;
        }

        public void Throw(Vector3 dir, Vector3 angularVelocity)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.angularVelocity = angularVelocity;
            _rigidbody.AddForce(dir, ForceMode.Impulse);
        }
    }
}