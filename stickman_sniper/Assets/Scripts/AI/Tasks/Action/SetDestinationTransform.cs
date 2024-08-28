using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Unity/NavMeshAgent")]
public class SetDestinationTransform : Action
{
    public SharedNavMeshAgent NavMeshAgent;
    public SharedTransform DestinationTransform;

    public override void OnStart()
    {
        if (NavMeshAgent == null)
        {
            Debug.LogError("SetDestinationTransform: agent is empty");
            return;
        }
    }

    public override TaskStatus OnUpdate()
    {
        //NavMeshPath navMeshPath = new();
        //if (!NavMeshAgent.Value.CalculatePath(DestinationTransform.Value.position, navMeshPath) || navMeshPath.status != NavMeshPathStatus.PathComplete)
        //{
        //    return TaskStatus.Failure;
        //}

        NavMeshAgent.Value.SetDestination(DestinationTransform.Value.position);
        return TaskStatus.Success;
    }
}
