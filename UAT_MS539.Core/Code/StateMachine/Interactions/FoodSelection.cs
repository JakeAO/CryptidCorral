using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class FoodSelection : IInteraction
    {
        public readonly IReadOnlyList<Food.Food> Options;
        public readonly Action<Food.Food> OptionSelectedHandler;

        public FoodSelection(IReadOnlyList<Food.Food> options, Action<Food.Food> optionSelectedHandler)
        {
            Options = options;
            OptionSelectedHandler = optionSelectedHandler;
        }
    }
}