using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class TownNurseryState : IState
    {
        public string LocationLocId => "Location/Town/Nursery";
        public string TimeLocId => "Time/Day";

        private Context _sharedContext;
        private PlayerData _playerData;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _playerData = context.Get<PlayerData>();
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
            if (_playerData.ActiveCryptid != null)
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Nursery/FullCorral"),
                    new Option("Button/Adopt", OnAdoptSelected),
                    new Option("Button/Exit", OnExitSelected)
                });
            }
            else
            {
                Random rand = new Random();
                List<RunePattern> selectableRunePatterns = new List<RunePattern>(5);

                var knownRunePatterns = _sharedContext.Get<RunePatternDatabase>().KnownRunePatterns;
                while (selectableRunePatterns.Count < 5)
                {
                    int index = rand.Next(knownRunePatterns.Count);
                    RunePattern pattern = knownRunePatterns[index];
                    if (_playerData.ConsumedRunePatterns.Contains(pattern.RunicHash))
                        continue;

                    selectableRunePatterns.Add(pattern);
                }

                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Nursery/SelectRune"),
                    new RunePatternSelection(selectableRunePatterns, OnAdoptRuneSelected),
                });
            }
        }

        private void OnAdoptRuneSelected(RunePattern runePattern)
        {
            var newCryptid = CryptidUtilities.CreateCryptid(
                runePattern.RunicHash,
                _sharedContext.Get<SpeciesDatabase>(),
                _sharedContext.Get<ColorDatabase>());

            _playerData.ActiveCryptid = newCryptid;
            _playerData.ConsumedRunePatterns.Add(runePattern.RunicHash);

            var locDatabase = _sharedContext.Get<LocDatabase>();
            var cryptidSpeciesName = locDatabase.Localize(newCryptid.Species.NameId);

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new DisplayCryptid(newCryptid),
                new Dialog("Town/Nursery/AdoptResult", new KeyValuePair<string, string>("{cryptidSpecies}", cryptidSpeciesName)),
                new Option("Button/Exit", OnExitSelected)
            });
        }

        private void OnExitSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TownMainState>();
        }
    }
}