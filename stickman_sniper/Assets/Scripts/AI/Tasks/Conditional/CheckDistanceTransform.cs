using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Utility")]
public class CheckDistanceTransform : Conditional
{
    public SharedNavMeshAgent Agent;
    public SharedTransform TargetTransform;
    public float Distance;

    public override TaskStatus OnUpdate()
    {
        TaskStatus result = TaskStatus.Running;
        if (Vector3.Distance(Agent.Value.transform.position, TargetTransform.Value.position) <= Distance)
        {
            result = TaskStatus.Success;
        }

        return result;
    }
}
