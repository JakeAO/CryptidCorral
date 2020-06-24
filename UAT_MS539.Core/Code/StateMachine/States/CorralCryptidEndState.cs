using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class CorralCryptidEndState : IState
    {
        public string LocationLocId => "Location/Corral";
        public string TimeLocId => "Time/Evening";

        private Context _sharedContext;
        private LocDatabase _locDatabase;
        private InteractionEventRaised _interactionSignal;
        private PlayerData _playerData;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _locDatabase = context.Get<LocDatabase>();
            _playerData = context.Get<PlayerData>();
            _interactionSignal = context.Get<InteractionEventRaised>();
        }

        public void PerformContent(Context context)
        {
            _interactionSignal.Fire(new IInteraction[]
            {
                new Dialog("Corral/Night/End/Goodbye",
                    new KeyValuePair<string, string>("{species}", _locDatabase.Localize(_playerData.ActiveCryptid.Species.NameId))),
                new Option("Button/Ready", OnReadySelected),
            });
        }

        private void OnReadySelected()
        {
            Cryptid.Cryptid goneCryptid = _playerData.ActiveCryptid;

            _playerData.ActiveCryptid = null;
            _playerData.DnaSampleInventory.Add(new CryptidDnaSample(goneCryptid));

            _interactionSignal.Fire(new IInteraction[]
            {
                new UpdatePlayerData(_playerData),
                new Dialog("Corral/Night/End/Gone",
                    new KeyValuePair<string, string>("{species}", _locDatabase.Localize(goneCryptid.Species.NameId)),
                    new KeyValuePair<string, string>("{age}", goneCryptid.AgeInDays.ToString())),
                new Option("Button/EndDay", OnEndDaySelected),
            });
        }

        private void OnEndDaySelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralMorningState>();
        }
        
        public void PerformTeardown(Context context, IState nextState)
        {
            context.Get<PlayerDataUtility>().TrySave(_playerData);
        }
    }
}