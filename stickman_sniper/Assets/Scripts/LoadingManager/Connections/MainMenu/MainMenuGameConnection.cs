using DW.StateMachine;

public class MainMenuGameConnection : BaseConnection
{
    public MainMenuGameConnection(IState inState, IState outState) : base(inState, outState)
    {
    }

    public override bool TestConnection()
    {
        return CheckBlackboardVariable(BlackboardConstants.MainMenuReadyBool, (bool x) => x == true);
    }
}
