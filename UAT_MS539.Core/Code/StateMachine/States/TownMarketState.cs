using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class TownMarketState : IState
    {
        public string LocationLocId => "Location/Town/Market";
        public string TimeLocId => "Time/Day";

        private Context _sharedContext;
        private PlayerData _playerData;

        private static (uint, List<(Food.Food, uint)>) _selectionForDay = (0, null);

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _playerData = context.Get<PlayerData>();

            if (_selectionForDay == default ||
                _selectionForDay.Item1 != _playerData.Day ||
                _selectionForDay.Item2 == null)
            {
                Random random = new Random();
                FoodDatabase foodDatabase = context.Get<FoodDatabase>();

                _selectionForDay = (_playerData.Day, new List<(Food.Food, uint)>(10));
                for (int i = 0; i < _selectionForDay.Item2.Capacity; i++)
                {
                    float randPerc = (float) random.NextDouble();
                    string foodId = foodDatabase.FoodSpawnRate.Evaluate(randPerc);
                    FoodDefinition foodDef = foodDatabase.FoodById[foodId];
                    Food.Food newFood = FoodUtilities.CreateFood(foodDef);
                    _selectionForDay.Item2.Add((newFood, GetFoodBuyCost(newFood)));
                }
            }
        }

        public void PerformContent(Context context)
        {
            MainPrompt();
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private uint GetFoodBuyCost(Food.Food food) => (uint) Math.Ceiling(food.TotalFoodQuality);
        private uint GetFoodSellCost(Food.Food food) => (uint) Math.Ceiling(food.TotalFoodQuality * 0.7f);

        private void MainPrompt()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new UpdatePlayerData(_playerData), 
                new Dialog("Town/Market/Welcome"),
                new Option("Button/Buy", OnBuySelected),
                new Option("Button/Sell", OnSellSelected),
                new Option("Button/Exit", OnExitSelected)
            });
        }

        private void OnBuySelected()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Town/Market/BuyPrompt"),
                new BuySellSelection(true, _selectionForDay.Item2, OnBuySelected_FoodSelected),
                new Option("Button/Cancel", MainPrompt),
            });
        }

        private void OnBuySelected_FoodSelected((Food.Food, uint) foodCostPair)
        {
            if (foodCostPair != default &&
                foodCostPair.Item2 <= _playerData.Coins)
            {
                _playerData.Coins -= foodCostPair.Item2;
                _playerData.FoodInventory.Add(foodCostPair.Item1);

                _selectionForDay.Item2.Remove(foodCostPair);

                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new UpdatePlayerData(_playerData),
                });
            }

            OnBuySelected();
        }

        private void OnSellSelected()
        {
            var sellOptions = _playerData.FoodInventory.ConvertAll(x => (x, GetFoodSellCost(x)));

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Town/Market/SellPrompt"),
                new BuySellSelection(false, sellOptions, OnSellSelected_FoodSelected),
                new Option("Button/Cancel", MainPrompt),
            });
        }

        private void OnSellSelected_FoodSelected((Food.Food, uint) foodCostPair)
        {
            _playerData.FoodInventory.Remove(foodCostPair.Item1);
            _playerData.Coins += foodCostPair.Item2;

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new UpdatePlayerData(_playerData),
            });

            OnSellSelected();
        }

        private void OnExitSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TownMainState>();
        }
    }
}