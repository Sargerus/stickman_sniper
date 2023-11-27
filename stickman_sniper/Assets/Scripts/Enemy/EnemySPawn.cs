using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySPawn : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position;;
        Gizmos.DrawLine(from, transform.position + transform.forward);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
