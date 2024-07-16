using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("EnemyAnimatorController")]
public class SetEnemyAnimatorValues : Action
{
    public SharedEnemyAnimatorController enemyAnimatorController;

    public bool SetWalk;
    public bool IsWalk;

    public bool SetShooting;
    public bool IsShooting;

    public override TaskStatus OnUpdate()
    {
        if (SetWalk)
            enemyAnimatorController.Value.SetWalk(IsWalk);

        if(SetShooting)
            enemyAnimatorController.Value.SetShoot(IsShooting);

        return TaskStatus.Success;
    }
}
