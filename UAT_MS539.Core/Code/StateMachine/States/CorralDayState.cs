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
        private readonly Random _random = null;

        private Context _sharedContext;
        private Cryptid.Cryptid _cryptid;
        private DailyTrainingData _trainingData;
        
        private IReadOnlyList<TrainingRegimen> _availableRegimens;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _cryptid = context.Get<PlayerData>().ActiveCryptid;
            _trainingData = context.Get<DailyTrainingData>();

            var trainingDatabase = context.Get<TrainingDatabase>();
            _availableRegimens = new List<TrainingRegimen>(); // TODO
        }

        public void PerformContent(Context context)
        {
            context.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("[TEMP] Training day welcome, prompt for input."),
                new Option("Button/Train", PromptForTraining),
                new Option("Button/Rest", OnRestSelected)
            });
        }

        private void PromptForTraining()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("[TEMP] Select a training regimen."),
                //new TrainingSelection(_availableRegimens, OnTrainingSelected),
                new Option("Button/AllDone", OnAllDoneSelected)
            });
        }

        private void OnTrainingSelected(TrainingRegimen regimen)
        {
            _cryptid.CurrentStamina -= regimen.StaminaCost;

            for (var i = 0; i < (int) EPrimaryStat._Count; i++)
            {
                // TODO: Use Luck and stuff here.
                var successRate = (float) _random.NextDouble();

                var guaranteedExp = 0u;
                var randomExp = 0u;

                regimen.GuaranteedStatIncrease.TryGetValue((EPrimaryStat) i, out guaranteedExp);
                if (regimen.RandomStatIncreases.TryGetValue((EPrimaryStat) i, out var randomExpTable))
                    randomExp = randomExpTable.Evaluate(successRate);

                _trainingData.Points[i] = guaranteedExp + randomExp;
            }

            // TODO: Show training results

            PromptForTraining();
        }

        private void OnRestSelected()
        {
            // TODO REST STUFF
            // INCREASE MORALE SOME?
            // RESTORE HEALTH SOME?

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("[TEMP] Your Cryptid got some rest, it's happier now."),
                new Option("Button/AllDone", OnAllDoneSelected)
            });
        }

        private void OnAllDoneSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}