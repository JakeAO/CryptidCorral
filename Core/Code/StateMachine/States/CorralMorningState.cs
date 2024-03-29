﻿using System;
using System.Collections.Generic;
using Core.Code.Cryptid;
using Core.Code.Food;
using Core.Code.StateMachine.Interactions;
using Core.Code.StateMachine.Signals;
using Core.Code.Utility;

namespace Core.Code.StateMachine.States
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

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new UpdatePlayerData(_playerData)
            });

            ActivityPrompt();
        }

        private void ActivityPrompt()
        {
            var interactions = new List<IInteraction>
            {
                new Dialog("Corral/Morning/ActivityPrompt"),
                new Option("Button/Rest", OnRestSelected),
                new Option("Button/ToTown", OnTownSelected),
            };

            if (_playerData.ActiveCryptid != null) interactions.Add(new Option("Button/Train", OnTrainingSelected));

            if (_playerData.Day % 7 == 6) interactions.Add(new Option("Button/ToColiseum", OnColiseumSelected));

            _sharedContext.Get<InteractionEventRaised>().Fire(interactions);
        }

        private void OnRestSelected()
        {
            void OnNextSelected()
            {
                _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
            }

            if (_playerData.ActiveCryptid != null)
            {
                // Improve Health
                _playerData.ActiveCryptid.CurrentHealth += (uint) Math.Round(_playerData.ActiveCryptid.MaxHealth * 0.25f);
                _playerData.ActiveCryptid.CurrentHealth = (uint) Math.Min(_playerData.ActiveCryptid.MaxHealth, _playerData.ActiveCryptid.CurrentHealth);

                // Improve Morale
                if (_playerData.ActiveCryptid.CurrentMorale < 10)
                {
                    _playerData.ActiveCryptid.CurrentMorale += 10;
                }
                else
                {
                    _playerData.ActiveCryptid.CurrentMorale += (uint) Math.Round(_playerData.ActiveCryptid.CurrentMorale * 0.1f);
                }

                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Corral/Day/RestResult"),
                    new Option("Button/Next", OnNextSelected)
                });
            }
            else
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Corral/Day/RestResultNoCryptid"),
                    new Option("Button/Next", OnNextSelected)
                });
            }
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