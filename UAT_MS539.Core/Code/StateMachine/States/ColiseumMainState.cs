using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class ColiseumMainState : IState
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
            context.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Coliseum/Welcome"),
                //new OpponentSelection(listOfOpponents),
                new Option("Button/GoHome", OnGoHomeSelected)
            });
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void OnOpponentSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<ColiseumBattleState>();
        }

        private void OnGoHomeSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}