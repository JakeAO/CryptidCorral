namespace Core.Code.StateMachine.States
{
    public class NilState : IState
    {
        public string LocationLocId => string.Empty;
        public string TimeLocId => string.Empty;

        public void PerformSetup(Context context, IState previousState)
        {
            // Intentionally left blank
        }

        public void PerformContent(Context context)
        {
            // Intentionally left blank
        }

        public void PerformTeardown(Context context, IState nextState)
        {
            // Intentionally left blank
        }
    }
}