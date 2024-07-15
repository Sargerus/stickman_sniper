using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Unity/NavMeshAgent")]
public class SetDestinationTransform : Action
{
    public SharedNavMeshAgent NavMeshAgent;
    public SharedTransform DestinationTransform;

    public override void OnStart()
    {
        if(NavMeshAgent == null)
        {
            Debug.LogError("SetDestinationTransform: agent is empty");
            return;
        }
    }

    public override TaskStatus OnUpdate()
    {
        NavMeshAgent.Value.SetDestination(DestinationTransform.Value.position);
        return TaskStatus.Success;
    }
}
