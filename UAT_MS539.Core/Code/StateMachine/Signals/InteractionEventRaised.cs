using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.Signals
{
    public class InteractionEventRaised : ISignal<IReadOnlyCollection<IInteraction>>
    {
        private event Action<IReadOnlyCollection<IInteraction>> EventHandlers;

        public void Fire(IReadOnlyCollection<IInteraction> value) => EventHandlers?.Invoke(value);
        public void Listen(Action<IReadOnlyCollection<IInteraction>> callback) => EventHandlers += callback;
        public void Unlisten(Action<IReadOnlyCollection<IInteraction>> callback) => EventHandlers -= callback;
    }
}