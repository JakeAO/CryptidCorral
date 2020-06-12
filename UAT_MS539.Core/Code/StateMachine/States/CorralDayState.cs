using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Training;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class CorralDayState : IState
    {
        public string LocationLocId => "Location/Corral";
        public string TimeLocId => "Time/Day";

        private readonly Random _random = new Random();

        private Context _sharedContext;
        private PlayerData _playerData;
        private Cryptid.Cryptid _cryptid;
        private DailyTrainingData _trainingData;

        private static (uint, List<TrainingRegimen>) _selectionForDay = (0, null);

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _playerData = context.Get<PlayerData>();
            _cryptid = _playerData.ActiveCryptid;
            _trainingData = context.Get<DailyTrainingData>();

            if (_selectionForDay == default ||
                _selectionForDay.Item1 != _playerData.Day ||
                _selectionForDay.Item2 == null)
            {
                TrainingDatabase trainingDatabase = context.Get<TrainingDatabase>();

                _selectionForDay = (_playerData.Day, new List<TrainingRegimen>(5));
                for (int i = 0; i < _selectionForDay.Item2.Capacity; i++)
                {
                    float randPerc = (float) _random.NextDouble();
                    string trainingId = trainingDatabase.TrainingSpawnRate.Evaluate(randPerc);
                    TrainingRegimen regimen = trainingDatabase.TrainingById[trainingId];

                    // Ignore duplicates
                    if (_selectionForDay.Item2.Contains(regimen))
                    {
                        i--;
                        continue;
                    }
                    _selectionForDay.Item2.Add(regimen);
                }
            }
        }

        public void PerformContent(Context context)
        {
            PromptForTraining();
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void PromptForTraining()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Corral/Day/TrainingPrompt"),
                new Dialog("Corral/Day/RemainingStamina", new KeyValuePair<string, string>("{stamina}", _cryptid.CurrentStamina.ToString())),
                new TrainingSelection(_selectionForDay.Item2, OnTrainingSelected),
                new Option("Button/AllDone", OnAllDoneSelected)
            });
        }

        private void OnTrainingSelected(TrainingRegimen regimen)
        {
            if (regimen.StaminaCost <= _cryptid.CurrentStamina)
            {
                _cryptid.CurrentStamina -= regimen.StaminaCost;

                uint[] gainedPoints = new uint[(int) EPrimaryStat._Count];
                for (int i = 0; i < (int) EPrimaryStat._Count; i++)
                {
                    var guaranteedExp = 0u;
                    var randomExp = 0u;

                    regimen.GuaranteedStatIncrease.TryGetValue((EPrimaryStat) i, out guaranteedExp);
                    if (regimen.RandomStatIncreases.TryGetValue((EPrimaryStat) i, out var randomExpTable))
                    {
                        // Luck stat can potentially result in up to 5 stat growth samples
                        uint sampleCount = 1;
                        while (sampleCount < 5 &&
                               _random.Next((int) CryptidUtilities.MAX_STAT_VAL) < _cryptid.PrimaryStats[(int) EPrimaryStat.Luck])
                        {
                            sampleCount++;
                        }

                        for (int j = 0; j < sampleCount; j++)
                        {
                            float successRate = (float) _random.NextDouble();
                            uint result = randomExpTable.Evaluate(successRate);
                            if (result > randomExp)
                                randomExp = result;
                        }
                    }

                    gainedPoints[i] = guaranteedExp + randomExp;
                    _trainingData.Points[i] += gainedPoints[i];
                }

                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new DisplayCryptid(_cryptid),
                    new DisplayTrainingResults(gainedPoints),
                    new Option("Button/Next", PromptForTraining)
                });
            }
            else
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Corral/Day/TrainingPromptTooTired"),
                    new Dialog("Corral/Day/RemainingStamina", new KeyValuePair<string, string>("{stamina}", _cryptid.CurrentStamina.ToString())),
                    new TrainingSelection(_selectionForDay.Item2, OnTrainingSelected),
                    new Option("Button/AllDone", OnAllDoneSelected)
                });
            }
        }

        private void OnAllDoneSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}