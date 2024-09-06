using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cysharp.Threading.Tasks;

namespace StickmanSniper.AI
{
    [TaskCategory("EnemyBattleController")]
    public class EnemyBattleControllerShoot : Action
    {
        public SharedEnemyBattleController EnemyBattleController;
        public SharedVector3 Target;

        public override TaskStatus OnUpdate()
        {
            EnemyBattleController.Value.Shoot(Target.Value).Forget();
            return TaskStatus.Success;
        }
    }
}