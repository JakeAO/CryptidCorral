using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class CorralMorningState : IState
    {
        public string LocationLocId => "Location/Corral";
        public string TimeLocId => "Time/Morning";

        private Context _sharedContext;
        private PlayerData _playerData;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _playerData = context.Get<PlayerData>();
        }

        public void PerformContent(Context context)
        {
            if (_playerData.ActiveCryptid != null)
            {
                string speciesName = context.Get<LocDatabase>().Localize(_playerData.ActiveCryptid.Species.NameId);

                var finalFoodList = new List<Food.Food>(_playerData.FoodInventory);
                finalFoodList.Add(FoodUtilities.CreateBasicRation());

                context.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Corral/Morning/FeedingPrompt", new KeyValuePair<string, string>("{species}", speciesName)),
                    new FoodSelection(finalFoodList, OnFoodSelected)
                });
            }
            else
            {
                context.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Corral/Morning/NoCryptid"),
                    new Option("Button/Next", ActivityPrompt),
                });
            }
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void OnFoodSelected(Food.Food food)
        {
            var trainingData = new DailyTrainingData();
            _sharedContext.Set(trainingData);

            _playerData.FoodInventory.Remove(food);
            trainingData.Food = food;

            ActivityPrompt();
        }

        private void ActivityPrompt()
        {
            var interactions = new List<IInteraction>
            {
                new Dialog("Corral/Morning/ActivityPrompt"),
                new Option("Button/ToTown", OnTownSelected)
            };

            if (_playerData.ActiveCryptid != null) interactions.Add(new Option("Button/Train", OnTrainingSelected));

            if (_playerData.Day % 7 == 6) interactions.Add(new Option("Button/ToColiseum", OnColiseumSelected));

            _sharedContext.Get<InteractionEventRaised>().Fire(interactions);
        }

        private void OnTrainingSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralDayState>();
        }

        private void OnTownSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TownMainState>();
        }

        private void OnColiseumSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<ColiseumMainState>();
        }
    }
}