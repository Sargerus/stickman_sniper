using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Set Values")]
public class SetFloat : Action
{
    public SharedFloat VariableToSet;
    public SharedFloat Value;

    public override TaskStatus OnUpdate()
    {
        VariableToSet.Value = Value.Value;
        return TaskStatus.Success;
    }
}
