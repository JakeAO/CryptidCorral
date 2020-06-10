using System;
using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class DnaSampleSelection : IInteraction
    {
        public readonly IReadOnlyList<Cryptid.CryptidDnaSample> Options;
        public readonly Action<Cryptid.CryptidDnaSample> OptionSelectedHandler;

        public DnaSampleSelection(IReadOnlyList<Cryptid.CryptidDnaSample> options, Action<Cryptid.CryptidDnaSample> optionSelectedHandler)
        {
            Options = options;
            OptionSelectedHandler = optionSelectedHandler;
        }
    }
}