using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class BuySellSelection : IInteraction
    {
        public readonly IReadOnlyList<(Food.Food, uint)> Options;
        public readonly Action<Food.Food> OptionSelectedHandler;

        public BuySellSelection(IReadOnlyList<(Food.Food, uint)> options, Action<Food.Food> optionSelectedHandler)
        {
            Options = options;
            OptionSelectedHandler = optionSelectedHandler;
        }
    }
}