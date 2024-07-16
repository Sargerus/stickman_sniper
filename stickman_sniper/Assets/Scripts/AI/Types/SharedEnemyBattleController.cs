using BehaviorDesigner.Runtime;

public class SharedEnemyBattleController : SharedVariable<EnemyBattleController>
{
    public static implicit operator SharedEnemyBattleController(EnemyBattleController value)
    { return new SharedEnemyBattleController { Value = value }; }
}
