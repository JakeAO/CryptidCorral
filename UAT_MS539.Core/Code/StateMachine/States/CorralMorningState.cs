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
        private Context _sharedContext;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
        }

        public void PerformContent(Context context)
        {
            var playerData = context.Get<PlayerData>();

            var finalFoodList = new List<Food.Food>(playerData.FoodInventory);
            finalFoodList.Add(FoodUtilities.CreateBasicRation());

            context.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Corral/Morning/FeedingPrompt"),
                new FoodSelection(finalFoodList, OnFoodSelected)
            });
        }

        private void OnFoodSelected(Food.Food food)
        {
            var playerData = _sharedContext.Get<PlayerData>();
            var trainingData = new DailyTrainingData();
            _sharedContext.Set(trainingData);

            playerData.FoodInventory.Remove(food);
            trainingData.Food = food;

            var interactions = new List<IInteraction>
            {
                new Dialog("Corral/Morning/ActivityPrompt"),
                new Option("Corral/Morning/Activity/Town", OnTownSelected)
            };

            if (playerData.ActiveCryptid != null) interactions.Add(new Option("Corral/Morning/Activity/Training", OnTrainingSelected));

            if (playerData.Day % 7 == 0) interactions.Add(new Option("Corral/Morning/Activity/Coliseum", OnColiseumSelected));

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