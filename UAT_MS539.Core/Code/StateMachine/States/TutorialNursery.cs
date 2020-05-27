using System.Collections.Generic;
using System.Linq;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class TutorialNursery : IState
    {
        private Context _sharedContext;
        private IReadOnlyList<IReadOnlyCollection<IInteraction>> _dialogMap;
        private int _interactionIndex = -1;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;

            Option acknowledgeDialogOption = new Option("Button/Next", OnDialogAcknowledged);
            _dialogMap = new List<IReadOnlyCollection<IInteraction>>()
            {
                new List<IInteraction>() {new Dialog("Tutorial/Nursery/1"), acknowledgeDialogOption},
                new List<IInteraction>() {new Dialog("Tutorial/Nursery/2"), acknowledgeDialogOption},
                new List<IInteraction>() {new Dialog("Tutorial/Nursery/3"), new Option("Button/Nursery/Adopt", OnAdoptChosen)},
            };
        }

        public void PerformContent(Context context)
        {
            OnDialogAcknowledged();
        }

        private void OnDialogAcknowledged()
        {
            _interactionIndex++;

            if (_interactionIndex < _dialogMap.Count)
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(_dialogMap[_interactionIndex]);
            }
        }

        private void OnAdoptChosen()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Nursery/SelectRune"),
                new ListOption(_sharedContext.Get<RunePatternDatabase>().OrderedIds.Take(10).ToList(), OnAdoptRuneSelected)
            });
        }

        private void OnAdoptRuneSelected(string runicHash)
        {
            Cryptid.Cryptid newCryptid = CryptidUtilities.CreateCryptid(
                runicHash,
                _sharedContext.Get<SpeciesDatabase>(),
                _sharedContext.Get<PatternDatabase>(),
                _sharedContext.Get<ColorDatabase>());

            PlayerData playerData = _sharedContext.Get<PlayerData>();
            playerData.ActiveCryptid = newCryptid;
            playerData.ConsumedRunePatterns.Add(runicHash);
            
            LocDatabase locDatabase = _sharedContext.Get<LocDatabase>();
            string cryptidSpeciesName = locDatabase.Localize(newCryptid.Species.NameId);
            
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new DisplayCryptid(newCryptid),
                new Dialog("Tutorial/Nursery/4", new KeyValuePair<string, string>("{cryptidSpecies}", cryptidSpeciesName)),
                new Option("Button/Town/GoHome", OnGoHomeselected)
            });
        }

        private void OnGoHomeselected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}