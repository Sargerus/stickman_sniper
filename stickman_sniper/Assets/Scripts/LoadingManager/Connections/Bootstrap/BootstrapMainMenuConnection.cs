using DW.StateMachine;

public class BootstrapMainMenuConnection : BaseConnection
{
    public BootstrapMainMenuConnection(IState inState, IState outState) : base(inState, outState)
    {
    }

    public override bool TestConnection()
    {
        return CheckBlackboardVariable(BlackboardConstants.BootstrapReadyBool, (bool x) => x == true);
    }
}