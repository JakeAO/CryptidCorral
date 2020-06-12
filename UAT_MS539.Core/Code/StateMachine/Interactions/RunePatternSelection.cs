using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class RunePatternSelection : BaseSelectionInteraction<RunePattern>
    {
        public RunePatternSelection(IReadOnlyList<RunePattern> options, Action<RunePattern> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}