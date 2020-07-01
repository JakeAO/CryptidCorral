using Core.Code.StateMachine.Interactions;
using Core.Code.StateMachine.Signals;

namespace Core.Code.StateMachine.States
{
    public class TownMainState : IState
    {
        public string LocationLocId => "Location/Town";
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
                new Dialog("Town/Welcome"),
                new Option("Button/Market", OnMarketSelected),
                new Option("Button/Laboratory", OnLabSelected),
                new Option("Button/Nursery", OnNurserySelected),
                new Option("Button/GoHome", OnGoHomeSelected)
            });
        }

        private void OnLabSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TownLabState>();
        }

        private void OnNurserySelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TownNurseryState>();
        }

        private void OnMarketSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TownMarketState>();
        }

        private void OnGoHomeSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}