using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class ListOption : IInteraction
    {
        public readonly IReadOnlyList<string> Options;
        public readonly Action<string> OptionSelectedHandler;

        public ListOption(IReadOnlyList<string> options, Action<string> optionSelectedHandler)
        {
            Options = options;
            OptionSelectedHandler = optionSelectedHandler;
        }
    }
}