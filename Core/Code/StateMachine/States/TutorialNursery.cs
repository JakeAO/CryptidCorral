using System;
using System.Collections.Generic;
using Core.Code.Cryptid;
using Core.Code.Food;
using Core.Code.StateMachine.Interactions;
using Core.Code.StateMachine.Signals;
using Core.Code.Utility;

namespace Core.Code.StateMachine.States
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
                new Dialog("Town/Nursery/SelectRune"),
                new RunePatternSelection(selectableRunePatterns, OnAdoptRuneSelected)
            });
        }

        private void OnAdoptRuneSelected(RunePattern runePattern)
        {
            Cryptid.Cryptid CreateNewCryptid(RunePattern rp, PlayerData pd, SpeciesDatabase sd, ColorDatabase cd)
            {
                var c = CryptidUtilities.CreateCryptid(rp.RunicHash, sd, cd);
                pd.ActiveCryptid = c;
                pd.ConsumedRunePatterns.Add(rp.RunicHash);
                return c;
            }

            void CreateIntroFood(PlayerData pd, FoodDatabase fd, int c)
            {
                var random = new Random();
                for (var i = 0; i < c; i++)
                {
                    var foodId = fd.FoodSpawnRate.Evaluate((float) random.NextDouble());
                    var newFood = FoodUtilities.CreateFood(fd.FoodById[foodId]);
                    pd.FoodInventory.Add(newFood);
                }
            }

            PlayerData playerData = _sharedContext.Get<PlayerData>();
            SpeciesDatabase speciesDatabase = _sharedContext.Get<SpeciesDatabase>();
            ColorDatabase colorDatabase = _sharedContext.Get<ColorDatabase>();
            FoodDatabase foodDatabase = _sharedContext.Get<FoodDatabase>();

            Cryptid.Cryptid newCryptid = CreateNewCryptid(runePattern, playerData, speciesDatabase, colorDatabase);
            CreateIntroFood(playerData, foodDatabase, 7);
            
            var locDatabase = _sharedContext.Get<LocDatabase>();
            var cryptidSpeciesName = locDatabase.Localize(newCryptid.Species.NameId);

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new UpdatePlayerData(playerData),
                new Dialog("Tutorial/Nursery/4", new KeyValuePair<string, string>("{cryptidSpecies}", cryptidSpeciesName)),
                new Option("Button/GoHome", OnGoHomeSelected)
            });
        }

        private void OnGoHomeSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}