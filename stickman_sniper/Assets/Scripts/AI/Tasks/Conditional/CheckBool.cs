using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Check Values")]
public class CheckBool : Conditional
{
    public SharedBool Template;
    public bool TestValue;

    public override TaskStatus OnUpdate()
    {
        TaskStatus status = Template.Value == TestValue ? TaskStatus.Success : TaskStatus.Failure;
        return status;
    }
}
