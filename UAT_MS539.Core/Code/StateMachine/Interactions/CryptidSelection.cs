using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class CryptidSelection : BaseSelectionInteraction<Cryptid.Cryptid>
    {
        public CryptidSelection(IReadOnlyList<Cryptid.Cryptid> options, Action<Cryptid.Cryptid> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}