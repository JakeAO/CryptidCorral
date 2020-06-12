using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class BuySellSelection : BaseSelectionInteraction<(Food.Food, uint)>
    {
        public BuySellSelection(IReadOnlyList<(Food.Food, uint)> options, Action<(Food.Food, uint)> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}