using BehaviorDesigner.Runtime;
using UnityEngine;

namespace StickmanSniper.AI
{
    [RequireComponent(typeof(BehaviorTree))]
    public class GizmosShowRadius : MonoBehaviour
    {
        public string VariableName = "SearchRadius";
        private BehaviorTree _behaviorTree;

        private void Awake()
        {
            _behaviorTree = GetComponent<BehaviorTree>();
        }

        void OnDrawGizmosSelected()
        {

            if (_behaviorTree == null)
            {
                if (!TryGetComponent<BehaviorTree>(out _behaviorTree))
                    return;
            }

            SharedFloat radius = (SharedFloat)_behaviorTree.GetVariable(VariableName);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radius.Value);
        }
    }
}