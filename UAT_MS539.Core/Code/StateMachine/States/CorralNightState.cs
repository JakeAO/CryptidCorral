using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
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
                var maxHealth = _activeCryptid.SecondaryStats[(int) ESecondaryStat.Health];
                if (_activeCryptid.CurrentHealth < maxHealth)
                {
                    _activeCryptid.CurrentHealth += (uint) Math.Round(maxHealth * 0.1f);
                    _activeCryptid.CurrentHealth = Math.Min(playerData.ActiveCryptid.CurrentHealth, maxHealth);
                }

                // Restore (up to) 3 Stamina
                var maxStamina = _activeCryptid.SecondaryStats[(int) ESecondaryStat.Stamina];
                if (_activeCryptid.CurrentStamina < maxStamina)
                {
                    _activeCryptid.CurrentStamina += 3u;
                    _activeCryptid.CurrentStamina = Math.Min(playerData.ActiveCryptid.CurrentStamina, maxStamina);
                }
            }

            context.Get<PlayerDataUtility>().TrySave(playerData);
        }

        private void OnNextSelected()
        {
            _trainingData.CalculateExpIncreases(out var expIncreases, out var moraleIncrease);

            var primaryStatIncreases = new uint[(int) EPrimaryStat._Count];
            for (var i = 0; i < (int) EPrimaryStat._Count; i++)
            {
                var totalExp = _activeCryptid.PrimaryStatExp[i] + expIncreases[i];
                primaryStatIncreases[i] = totalExp / 100;
                var remainingExp = totalExp % 100;

                _activeCryptid.PrimaryStats[i] += primaryStatIncreases[i];
                _activeCryptid.PrimaryStatExp[i] = remainingExp;
            }

            var healthIncrease = primaryStatIncreases[(int) EPrimaryStat.Vitality] * 10;
            _activeCryptid.SecondaryStats[(int) ESecondaryStat.Health] += healthIncrease;
            _activeCryptid.CurrentHealth += healthIncrease;

            var secondaryStatIncreases = new uint[(int) ESecondaryStat._Count];
            secondaryStatIncreases[(int) ESecondaryStat.Health] = healthIncrease;

            _activeCryptid.CurrentMorale += moraleIncrease;

            _sharedContext.Clear<DailyTrainingData>();
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new DisplayCryptidAdvancement(_activeCryptid, primaryStatIncreases, secondaryStatIncreases),
                new Option("Button/EndDay", OnEndDaySelected)
            });
        }

        private void OnEndDaySelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralMorningState>();
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