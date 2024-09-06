using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace StickmanSniper.AI
{
    [TaskCategory("Set Values")]
    public class SetTransform : Action
    {
        public SharedTransform Reference;
        public Transform Value;

        public override TaskStatus OnUpdate()
        {
            Reference.Value = Value;
            return TaskStatus.Success;
        }
    }
}