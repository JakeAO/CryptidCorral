using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class FoodSelection : BaseSelectionInteraction<Food.Food>
    {
        public FoodSelection(IReadOnlyList<Food.Food> options, Action<Food.Food> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}