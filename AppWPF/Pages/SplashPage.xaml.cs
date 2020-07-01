using System.Collections.Generic;
using System.Windows.Controls;
using AppWPF.Code;
using Core.Code.StateMachine;
using Core.Code.StateMachine.Interactions;
using Core.Code.Utility;

namespace AppWPF.Pages
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