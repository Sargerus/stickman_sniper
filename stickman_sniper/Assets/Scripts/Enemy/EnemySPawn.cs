using UnityEngine;

namespace StickmanSniper.Enemy
{
    public class EnemySPawn : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 from = transform.position; ;
            Gizmos.DrawLine(from, transform.position + transform.forward);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.4f);
        }
    }
}