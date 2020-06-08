namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class ColiseumBattleState : IState
    {
        public string LocationLocId => "Location/Coliseum";
        public string TimeLocId => "Time/Day";

        private Context _sharedContext;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
        }

        public void PerformContent(Context context)
        {
            // battle battle battle
            // battle battle
            // battle
            // then results
            OnCombatFinished();
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void OnCombatFinished()
        {
            _sharedContext.Get<StateMachine>().ChangeState<ColiseumResultsState>();
        }
    }
}