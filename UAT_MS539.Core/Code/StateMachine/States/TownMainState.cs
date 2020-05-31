using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class TownMainState : IState
    {
        private Context _sharedContext;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
        }

        public void PerformContent(Context context)
        {
            MainPrompt();
        }

        private void MainPrompt()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("[TEMP] Town welcome, prompt for input."),
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