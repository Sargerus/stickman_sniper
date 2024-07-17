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
        //workaround(animation rigging required)
        Vector3 rot = new(Target.Value.x, RotTransform.Value.position.y, Target.Value.z);

        RotTransform.Value.rotation = Quaternion.LookRotation((rot - RotTransform.Value.position).normalized);
        return TaskStatus.Running;
    }
}
