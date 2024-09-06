using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace StickmanSniper.AI
{
    [TaskCategory("Unity/NavMeshAgent")]
    public class CheckNavMeshPathReachable : Conditional
    {
        public SharedNavMeshAgent agent;
        public SharedVector3 target;

        public override TaskStatus OnUpdate()
        {
            TaskStatus result = TaskStatus.Failure;

            //unity workaround
            //fix it once you have better solution
            NavMeshPath navMeshPath = new NavMeshPath();
            if (agent.Value.CalculatePath(target.Value, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                result = TaskStatus.Success;
            }

            return result;
        }
    }
}