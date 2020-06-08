using System.Collections.Generic;
using UAT_MS539.Core.Code.StateMachine.Interactions;

namespace UAT_MS539.Code
{
    public interface IInteractionHandler
    {
        void HandleInteraction(IReadOnlyCollection<IInteraction> interactions);
    }
}