using System;
using System.Collections.Generic;
using Core.Code.Training;

namespace Core.Code.StateMachine.Interactions
{
    public class TrainingSelection : BaseSelectionInteraction<TrainingRegimen>
    {
        public TrainingSelection(IReadOnlyList<TrainingRegimen> options, Action<TrainingRegimen> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}