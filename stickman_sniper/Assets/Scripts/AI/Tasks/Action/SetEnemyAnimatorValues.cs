using BehaviorDesigner.Runtime.Tasks;

namespace StickmanSniper.AI
{
    [TaskCategory("EnemyAnimatorController")]
    public class SetEnemyAnimatorValues : Action
    {
        public SharedEnemyAnimatorController enemyAnimatorController;

        public bool SetWalk;
        public bool IsWalk;

        public bool SetShooting;
        public bool IsShooting;

        public bool SetCrouching;
        public bool IsCrouching;

        public override TaskStatus OnUpdate()
        {
            if (SetWalk)
                enemyAnimatorController.Value.SetWalk(IsWalk);

            if (SetShooting)
                enemyAnimatorController.Value.SetShoot(IsShooting);

            if (SetCrouching)
                enemyAnimatorController.Value.SetCroushing(IsCrouching);

            return TaskStatus.Success;
        }
    }
}