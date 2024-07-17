using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class FindNewWanderingPoint : Action
{
    public SharedFloat SearchRadius;
    public SharedNavMeshAgent Agent;
    public SharedInt AreaMask;
    public SharedVector3 MovePosition;

    public override TaskStatus OnUpdate()
    {
        TaskStatus status = TaskStatus.Running;

        Vector3 randomPoint = Agent.Value.transform.position + 
            new Vector3(Random.Range(-SearchRadius.Value, SearchRadius.Value), Random.Range(0, SearchRadius.Value), Random.Range(-SearchRadius.Value, SearchRadius.Value));

        Debug.LogWarning(randomPoint);

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 1f, 1 << AreaMask.Value))
        {
            MovePosition.Value = hit.position;
            status = TaskStatus.Success;
        }

        return status;
    }
}
