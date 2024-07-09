using DW.StateMachine;

public class GameMainMenuConnection : BaseConnection
{
    public GameMainMenuConnection(IState inState, IState outState) : base(inState, outState)
    {
    }

    public override bool TestConnection()
    {
        return CheckBlackboardVariable(BlackboardConstants.GameOverBool, (bool x) => x == true);
    }
}