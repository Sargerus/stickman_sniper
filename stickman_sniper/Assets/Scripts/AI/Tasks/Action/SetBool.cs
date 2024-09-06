using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace StickmanSniper.AI
{
    [TaskCategory("Set Values")]
    public class SetBool : Action
    {
        public SharedBool VariableToSet;
        public bool Value;

        public override TaskStatus OnUpdate()
        {
            VariableToSet.Value = Value;
            return TaskStatus.Success;
        }
    }
}