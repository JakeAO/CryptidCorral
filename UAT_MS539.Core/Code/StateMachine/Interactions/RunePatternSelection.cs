using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class RunePatternSelection : IInteraction
    {
        public readonly IReadOnlyList<RunePattern> Options;
        public readonly Action<RunePattern> OptionSelectedHandler;

        public RunePatternSelection(IReadOnlyList<RunePattern> options, Action<RunePattern> optionSelectedHandler)
        {
            Options = options;
            OptionSelectedHandler = optionSelectedHandler;
        }
    }
}