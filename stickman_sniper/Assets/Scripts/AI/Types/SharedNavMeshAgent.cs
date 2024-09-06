using BehaviorDesigner.Runtime;
using UnityEngine.AI;

namespace StickmanSniper.AI
{
    [System.Serializable]
    public class SharedNavMeshAgent : SharedVariable<NavMeshAgent>
    {
        public static implicit operator SharedNavMeshAgent(NavMeshAgent value)
        { return new SharedNavMeshAgent { Value = value }; }
    }
}