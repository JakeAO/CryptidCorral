using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class TownMarketState : IState
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
                new Dialog("[TEMP] Market welcome, prompt for input."),
                new Option("Button/Buy", OnBuySelected),
                new Option("Button/Sell", OnSellSelected),
                new Option("Button/Exit", OnExitSelected)
            });
        }

        private void OnBuySelected()
        {
            // TODO
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("= = = Not Implemented = = =")
            });
            MainPrompt();
        }

        private void OnSellSelected()
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