using Core.Code.StateMachine.Interactions;
using Core.Code.StateMachine.Signals;
using Core.Code.Utility;

namespace Core.Code.StateMachine.States
{
    public class SplashState : IState
    {
        public string LocationLocId => string.Empty;
        public string TimeLocId => string.Empty;

        private Context _sharedContext;

        public bool SaveFound { get; private set; }

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
        }

        public void PerformContent(Context context)
        {
            SaveFound = context.Get<PlayerDataUtility>().TryLoad(out var playerData);

            if (!SaveFound)
                playerData = new PlayerData();

            context.Set(playerData);

            context.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Option("Button/Play", Continue),
            });
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void Continue()
        {
            if (SaveFound)
                _sharedContext.Get<StateMachine>().ChangeState<CorralMorningState>();
            else
                _sharedContext.Get<StateMachine>().ChangeState<TutorialCorral>();
        }
    }
}