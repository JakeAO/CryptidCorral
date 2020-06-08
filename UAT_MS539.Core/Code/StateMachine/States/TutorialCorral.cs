using System.Collections.Generic;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class TutorialCorral : IState
    {
        public string LocationLocId => "Location/Corral";
        public string TimeLocId => "Time/Morning";

        private Context _sharedContext;

        private IReadOnlyList<IReadOnlyCollection<IInteraction>> _introductionDialogEvents;
        private int _dialogIndex = -1;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;

            var acknowledgeDialogOption = new Option("Button/Next", OnDialogAcknowledged);
            _introductionDialogEvents = new List<List<IInteraction>>
            {
                new List<IInteraction> {new Dialog("Tutorial/Corral/1"), acknowledgeDialogOption},
                new List<IInteraction> {new Dialog("Tutorial/Corral/2"), acknowledgeDialogOption},
                new List<IInteraction> {new Dialog("Tutorial/Corral/3"), acknowledgeDialogOption},
                new List<IInteraction> {new Dialog("Tutorial/Corral/4"), acknowledgeDialogOption},
                new List<IInteraction> {new Dialog("Tutorial/Corral/5"), new Option("Tutorial/LetsGo", OnLastDialogAcknowledged)}
            };
        }

        /// <summary>
        ///     Show user dialogs (tutorial lite), then transition to Day 0 game-play
        /// </summary>
        public void PerformContent(Context context)
        {
            OnDialogAcknowledged();
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void OnDialogAcknowledged()
        {
            _dialogIndex++;

            if (_dialogIndex < _introductionDialogEvents.Count)
                _sharedContext.Get<InteractionEventRaised>().Fire(_introductionDialogEvents[_dialogIndex]);
            else
                // Should never hit this block
                OnLastDialogAcknowledged();
        }

        private void OnLastDialogAcknowledged()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TutorialNursery>();
        }
    }
}