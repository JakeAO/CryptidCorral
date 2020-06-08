using System;
using System.Collections.Generic;
using System.Linq;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class TutorialNursery : IState
    {
        public string LocationLocId => "Location/Town/Nursery";
        public string TimeLocId => "Time/Day";

        private Context _sharedContext;
        
        private IReadOnlyList<IReadOnlyCollection<IInteraction>> _dialogMap;
        private int _interactionIndex = -1;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;

            var acknowledgeDialogOption = new Option("Button/Next", OnDialogAcknowledged);
            _dialogMap = new List<IReadOnlyCollection<IInteraction>>
            {
                new List<IInteraction> {new Dialog("Tutorial/Nursery/1"), acknowledgeDialogOption},
                new List<IInteraction> {new Dialog("Tutorial/Nursery/2"), acknowledgeDialogOption},
                new List<IInteraction> {new Dialog("Tutorial/Nursery/3"), new Option("Button/Adopt", OnAdoptChosen)}
            };
        }

        public void PerformContent(Context context)
        {
            OnDialogAcknowledged();
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void OnDialogAcknowledged()
        {
            _interactionIndex++;

            if (_interactionIndex < _dialogMap.Count) _sharedContext.Get<InteractionEventRaised>().Fire(_dialogMap[_interactionIndex]);
        }

        private void OnAdoptChosen()
        {
            Random rand = new Random();

            List<RunePattern> selectableRunePatterns = new List<RunePattern>(5);
            var knownRunePatterns = _sharedContext.Get<RunePatternDatabase>().KnownRunePatterns;
            for (int i = 0; i < selectableRunePatterns.Capacity; i++)
            {
                selectableRunePatterns.Add(knownRunePatterns[rand.Next(knownRunePatterns.Count)]);
            }

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Nursery/SelectRune"),
                new RunePatternSelection(selectableRunePatterns, OnAdoptRuneSelected)
            });
        }

        private void OnAdoptRuneSelected(RunePattern runePattern)
        {
            var newCryptid = CryptidUtilities.CreateCryptid(
                runePattern.RunicHash,
                _sharedContext.Get<SpeciesDatabase>(),
                _sharedContext.Get<ColorDatabase>());

            var playerData = _sharedContext.Get<PlayerData>();
            playerData.ActiveCryptid = newCryptid;
            playerData.ConsumedRunePatterns.Add(runePattern.RunicHash);

            var foodDatabase = _sharedContext.Get<FoodDatabase>();
            var random = new Random();
            for (var i = 0; i < 7; i++)
            {
                var randomFloat = (float) random.NextDouble();
                var foodId = foodDatabase.FoodSpawnRate.Evaluate(randomFloat);
                var foodDefinition = foodDatabase.FoodById[foodId];
                var newFood = FoodUtilities.CreateFood(foodDefinition);
                playerData.FoodInventory.Add(newFood);
            }

            var locDatabase = _sharedContext.Get<LocDatabase>();
            var cryptidSpeciesName = locDatabase.Localize(newCryptid.Species.NameId);

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new DisplayCryptid(newCryptid),
                new Dialog("Tutorial/Nursery/4", new KeyValuePair<string, string>("{cryptidSpecies}", cryptidSpeciesName)),
                new Option("Button/GoHome", OnGoHomeselected)
            });
        }

        private void OnGoHomeselected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}