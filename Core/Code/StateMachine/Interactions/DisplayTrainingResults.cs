namespace Core.Code.StateMachine.Interactions
{
    public class DisplayTrainingResults : IInteraction
    {
        public readonly uint[] TrainingPoints;

        public DisplayTrainingResults(uint[] trainingPoints)
        {
            TrainingPoints = trainingPoints;
        }
    }
}