using System;
using System.Collections.Generic;
using Core.Code.Cryptid;

namespace Core.Code.StateMachine.Interactions
{
    public class DnaSampleSelection : BaseSelectionInteraction<CryptidDnaSample>
    {
        public DnaSampleSelection(IReadOnlyList<CryptidDnaSample> options, Action<CryptidDnaSample> optionSelectedHandler)
            : base(options, optionSelectedHandler)
        {
        }
    }
}