using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Training;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class TrainingSelection : BaseSelectionInteraction<TrainingRegimen>
    {
        public TrainingSelection(IReadOnlyList<TrainingRegimen> options, Action<TrainingRegimen> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}