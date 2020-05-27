namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class NilState : IState
    {
        public void PerformSetup(Context context, IState previousState)
        {
            // Intentionally left blank
        }

        public void PerformTeardown(Context context, IState nextState)
        {
            // Intentionally left blank
        }
    }
}