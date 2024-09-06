using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StickmanSniper.AI
{
    public class SampleNavMesh : MonoBehaviour
    {
        [Button]
        private void Sample(Transform transform, float maxDistance, int areaMask)
        {
            if (NavMesh.SamplePosition(transform.position, out _, maxDistance, 1 << NavMesh.GetAreaFromName("RedStickman")))
            {
                Debug.Log("Smapled!");
            }
        }
    }
}