﻿using Core.Code.Food;
using Core.Code.StateMachine.Signals;
using Core.Code.Training;
using Core.Code.Utility;

namespace Core.Code.StateMachine.States
{
    public class InitializationState : IState
    {
        public string LocationLocId => string.Empty;
        public string TimeLocId => string.Empty;

        public void PerformSetup(Context context, IState previousState)
        {
        }

        /// <summary>
        ///     Load all the databases and setup signals, then transition to the splash screen.
        /// </summary>
        public void PerformContent(Context context)
        {
            context.Set(new InteractionEventRaised(), false);

            context.Set(new LocDatabase("Source/locDatabase.json"));
            context.Set(new RuneDatabase("Source/runeDatabase.json"));
            context.Set(new SpeciesDatabase("Source/speciesDatabase.json"));
            context.Set(new ColorDatabase("Source/colorDatabase.json"));
            context.Set(new FoodDatabase("Source/foodDatabase.json"));
            context.Set(new TrainingDatabase("Source/trainingDatabase.json"));
            context.Set(new RunePatternDatabase());
            context.Set(new PlayerDataUtility(
                context.Get<FoodDatabase>(),
                context.Get<SpeciesDatabase>(),
                context.Get<ColorDatabase>()));

            context.Get<StateMachine>().ChangeState<SplashState>();
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }
    }
}