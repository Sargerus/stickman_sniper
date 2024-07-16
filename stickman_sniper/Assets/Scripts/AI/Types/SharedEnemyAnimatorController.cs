using BehaviorDesigner.Runtime;

public class SharedEnemyAnimatorController : SharedVariable<EnemyAnimatorController>
{
    public static implicit operator SharedEnemyAnimatorController(EnemyAnimatorController value)
    { return new SharedEnemyAnimatorController { Value = value }; }
}