using System;
using System.Collections.Generic;
using Core.Code.StateMachine.CombatEngine;

namespace Core.Code.StateMachine.Interactions
{
    public class ColiseumOpponentSelection : BaseSelectionInteraction<(Cryptid.Cryptid, EWeightClass)>
    {
        public ColiseumOpponentSelection(IReadOnlyList<(Cryptid.Cryptid, EWeightClass)> options, Action<(Cryptid.Cryptid, EWeightClass)> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}