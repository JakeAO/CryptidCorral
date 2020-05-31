namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class ColiseumBattleState : IState
    {
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

        private void OnCombatFinished()
        {
            _sharedContext.Get<StateMachine>().ChangeState<ColiseumResultsState>();
        }
    }
}