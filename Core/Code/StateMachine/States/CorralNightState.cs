﻿using System;
using System.Collections.Generic;
using Core.Code.Cryptid;
using Core.Code.StateMachine.Interactions;
using Core.Code.StateMachine.Signals;
using Core.Code.Utility;

namespace Core.Code.StateMachine.States
{
    public class CorralNightState : IState
    {
        public string LocationLocId => "Location/Corral";
        public string TimeLocId => "Time/Evening";

        private Context _sharedContext;
        private Cryptid.Cryptid _activeCryptid;
        private InteractionEventRaised _interactionSignal;
        private PlayerData _playerData;
        private DailyTrainingData _trainingData;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _playerData = context.Get<PlayerData>();
            _activeCryptid = _playerData.ActiveCryptid;
            _trainingData = context.Get<DailyTrainingData>();
            _interactionSignal = context.Get<InteractionEventRaised>();
        }

        public void PerformContent(Context context)
        {
            var endDayOption = new Option("Button/EndDay", OnEndDaySelected);
            if (_activeCryptid != null)
            {
                _interactionSignal.Fire(new IInteraction[]
                {
                    GetAgeWarning(),
                    GetMoraleWarning(),
                    _trainingData != null ? new Option("Button/Next", OnNextSelected) : endDayOption
                });
            }
            else
            {
                _interactionSignal.Fire(new IInteraction[]
                {
                    new Dialog("Corral/Night/Default"),
                    endDayOption
                });
            }
        }

        public void PerformTeardown(Context context, IState nextState)
        {
            var playerData = context.Get<PlayerData>();

            // Increment Day Counter
            playerData.Day++;

            if (_activeCryptid != null)
            {
                // Increment Cryptid Age
                _activeCryptid.AgeInDays++;

                // Restore (up to) 5% Morale
                var maxMorale = _activeCryptid.HiddenStats[(int) EHiddenStat.Morale];
                if (_activeCryptid.CurrentMorale < maxMorale)
                {
                    _activeCryptid.CurrentMorale += (uint) Math.Round(maxMorale * 0.05f);
                    _activeCryptid.CurrentMorale = Math.Min(playerData.ActiveCryptid.CurrentMorale, maxMorale);
                }

                // Restore (up to) 10% Health
                var maxHealth = _activeCryptid.MaxHealth;
                if (_activeCryptid.CurrentHealth < maxHealth)
                {
                    _activeCryptid.CurrentHealth += (uint) Math.Round(maxHealth * 0.1f);
                    _activeCryptid.CurrentHealth = Math.Min(playerData.ActiveCryptid.CurrentHealth, maxHealth);
                }

                // Restore All Stamina
                _activeCryptid.CurrentStamina = _activeCryptid.MaxStamina;
            }

            context.Get<PlayerDataUtility>().TrySave(playerData);
        }

        private void OnNextSelected()
        {
            _trainingData.CalculateExpIncreases(out var expIncreases, out var moraleIncrease);

            uint originalMaxHealth = _activeCryptid.MaxHealth;
            uint originalMaxStamina = _activeCryptid.MaxStamina;
            
            var primaryStatIncreases = new uint[(int) EPrimaryStat._Count];
            for (var i = 0; i < (int) EPrimaryStat._Count; i++)
            {
                var totalExp = _activeCryptid.PrimaryStatExp[i] + expIncreases[i];
                primaryStatIncreases[i] = totalExp / 100;
                var remainingExp = totalExp % 100;

                _activeCryptid.PrimaryStats[i] += primaryStatIncreases[i];
                _activeCryptid.PrimaryStatExp[i] = remainingExp;
            }

            uint healthIncrease = _activeCryptid.MaxHealth - originalMaxHealth;
            uint staminaIncrease = _activeCryptid.MaxStamina - originalMaxStamina;

            _activeCryptid.CurrentHealth += healthIncrease;
            _activeCryptid.CurrentMorale += moraleIncrease;

            _sharedContext.Clear<DailyTrainingData>();
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new DisplayCryptidAdvancement(_activeCryptid, primaryStatIncreases, healthIncrease, staminaIncrease),
                new Option("Button/EndDay", OnEndDaySelected)
            });
        }

        private void OnEndDaySelected()
        {
            if (_activeCryptid != null &&
                _activeCryptid.HiddenStats[(int) EHiddenStat.Lifespan] - _activeCryptid.AgeInDays == 0)
            {
                _sharedContext.Get<StateMachine>().ChangeState<CorralCryptidEndState>();
            }
            else
            {
                _sharedContext.Get<StateMachine>().ChangeState<CorralMorningState>();
            }
        }

        private Dialog GetAgeWarning()
        {
            LocDatabase locDatabase = _sharedContext.Get<LocDatabase>();
            var speciesFormatter = new KeyValuePair<string, string>("{species}", locDatabase.Localize(_playerData.ActiveCryptid.Species.NameId));

            var remainingLife = _activeCryptid.HiddenStats[(int) EHiddenStat.Lifespan] - _activeCryptid.AgeInDays;
            if (remainingLife < 1) return new Dialog("Corral/Night/AgeWarning/FinalDay", speciesFormatter);

            if (remainingLife < 3) return new Dialog("Corral/Night/AgeWarning/FinalDays", speciesFormatter);

            if (remainingLife < 7) return new Dialog("Corral/Night/AgeWarning/FinalWeek", speciesFormatter);

            if (remainingLife < 14) return new Dialog("Corral/Night/AgeWarning/FinalFortnight", speciesFormatter);

            return new Dialog("Corral/Night/AgeWarning/Healthy", speciesFormatter);
        }

        private Dialog GetMoraleWarning()
        {
            LocDatabase locDatabase = _sharedContext.Get<LocDatabase>();
            var speciesFormatter = new KeyValuePair<string, string>("{species}", locDatabase.Localize(_playerData.ActiveCryptid.Species.NameId));

            var morale = _activeCryptid.HiddenStats[(int) EHiddenStat.Morale];
            if (morale < 10) return new Dialog("Corral/Night/MoraleWarning/Tenth", speciesFormatter);

            if (morale < 25) return new Dialog("Corral/Night/MoraleWarning/Quarter", speciesFormatter);

            if (morale < 50) return new Dialog("Corral/Night/MoraleWarning/Half", speciesFormatter);

            return new Dialog("Corral/Night/MoraleWarning/Happy", speciesFormatter);
        }
    }
}