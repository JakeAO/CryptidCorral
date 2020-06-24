using System.Windows.Controls;
using UAT_MS539.Code;
using UAT_MS539.Core.Code.Utility;
using UAT_MS539.Core.Code.StateMachine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using System.Collections.Generic;

namespace UAT_MS539.Pages
{
    public partial class SplashPage : UserControl, IStateChangeHandler, IInteractionHandler
    {
        private Context _sharedContext = null;

        public SplashPage()
        {
            InitializeComponent();
        }

        public void OnStateChanged(IState state, Context sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public void HandleInteraction(IReadOnlyCollection<IInteraction> interactions)
        {
            foreach (IInteraction interaction in interactions)
            {
                switch (interaction)
                {
                    case Option option:
                    {
                        _buttonList.AddButton(
                            _sharedContext.Get<LocDatabase>().Localize(option.LocId),
                            () =>
                            {
                                _buttonList.ClearOptions();
                                _sharedContext.Get<AudioManager>().PlaySound(AudioManager.AudioEvent.Click);
                                option.ActionHandler?.Invoke();
                            });
                        break;
                    }
                }
            }
        }
    }
}