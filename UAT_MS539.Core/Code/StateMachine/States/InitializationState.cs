using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Training;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
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
            context.Set(new SpeciesDatabase("Source/speciesDatabase.json", "Source/speciesDropTable.json"));
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