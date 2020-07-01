using System;
using System.Collections.Generic;

namespace Core.Code.StateMachine.Interactions
{
    public class CryptidSelection : BaseSelectionInteraction<Cryptid.Cryptid>
    {
        public CryptidSelection(IReadOnlyList<Cryptid.Cryptid> options, Action<Cryptid.Cryptid> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}