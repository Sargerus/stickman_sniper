using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Utility")]
public class RotateTowardsVector : Action
{
    public SharedVector3 Target;
    public SharedTransform RotTransform;

    public override TaskStatus OnUpdate()
    {
        RotTransform.Value.rotation = Quaternion.LookRotation((Target.Value - RotTransform.Value.position).normalized);
        return TaskStatus.Running;
    }
}
