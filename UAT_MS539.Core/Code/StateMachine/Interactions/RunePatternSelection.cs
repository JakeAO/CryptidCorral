using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class RunePatternSelection : IInteraction
    {
        public readonly IReadOnlyList<string> Options;
        public readonly Action<string> OptionSelectedHandler;

        public RunePatternSelection(IReadOnlyList<string> options, Action<string> optionSelectedHandler)
        {
            Options = options;
            OptionSelectedHandler = optionSelectedHandler;
        }
    }
}