using System.Collections.Generic;
using Core.Code.StateMachine.Interactions;

namespace AppWPF.Code
{
    public interface IInteractionHandler
    {
        void HandleInteraction(IReadOnlyCollection<IInteraction> interactions);
    }
}