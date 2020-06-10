using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class CryptidSelection : IInteraction
    {
        public readonly IReadOnlyList<Cryptid.Cryptid> Options;
        public readonly Action<Cryptid.Cryptid> OptionSelectedHandler;

        public CryptidSelection(IReadOnlyList<Cryptid.Cryptid> options, Action<Cryptid.Cryptid> optionSelectedHandler)
        {
            Options = options;
            OptionSelectedHandler = optionSelectedHandler;
        }
    }
}