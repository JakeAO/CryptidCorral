using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class DnaSampleSelection : BaseSelectionInteraction<CryptidDnaSample>
    {
        public DnaSampleSelection(IReadOnlyList<CryptidDnaSample> options, Action<CryptidDnaSample> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}