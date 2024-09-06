using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace StickmanSniper.AI
{
    [TaskCategory("Utility")]
    public class CheckDistanceVector3 : Conditional
    {
        public SharedNavMeshAgent Agent;
        public SharedVector3 TargetPosition;
        public float Distance;

        public override TaskStatus OnUpdate()
        {
            TaskStatus result = TaskStatus.Running;
            if (Vector3.Distance(Agent.Value.transform.position, TargetPosition.Value) <= Distance)
            {
                result = TaskStatus.Success;
            }

            return result;
        }
    }
}