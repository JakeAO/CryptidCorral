using System;
using System.Collections.Generic;

namespace Core.Code.StateMachine.Interactions
{
    public class BaseSelectionInteraction<T> : IInteraction
    {
        public readonly IReadOnlyList<T> Options;
        public readonly Action<T> OptionSelectedHandler;

        public BaseSelectionInteraction(IReadOnlyList<T> options, Action<T> optionSelectedHandler)
        {
            Options = options;
            OptionSelectedHandler = optionSelectedHandler;
        }
    }
}