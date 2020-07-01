using System;
using System.Collections.Generic;
using Core.Code.Utility;

namespace Core.Code.StateMachine.Interactions
{
    public class RunePatternSelection : BaseSelectionInteraction<RunePattern>
    {
        public RunePatternSelection(IReadOnlyList<RunePattern> options, Action<RunePattern> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}