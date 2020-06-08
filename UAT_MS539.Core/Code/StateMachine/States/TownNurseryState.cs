using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class TownNurseryState : IState
    {
        public string LocationLocId => "Location/Town/Nursery";
        public string TimeLocId => "Time/Day";

        private Context _sharedContext;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
        }

        public void PerformContent(Context context)
        {
            MainPrompt();
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void MainPrompt()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Town/Nursery/Welcome"),
                new Option("Button/Adopt", OnAdoptSelected),
                new Option("Button/Exit", OnExitSelected)
            });
        }

        private void OnAdoptSelected()
        {
            // TODO
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("= = = Not Implemented = = =")
            });
            MainPrompt();
        }

        private void OnExitSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TownMainState>();
        }
    }
}