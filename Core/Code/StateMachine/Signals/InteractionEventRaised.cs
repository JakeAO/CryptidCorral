using System;
using System.Collections.Generic;
using Core.Code.StateMachine.Interactions;
using Core.Code.Utility;

namespace Core.Code.StateMachine.Signals
{
    public class InteractionEventRaised : ISignal<IReadOnlyCollection<IInteraction>>
    {
        private event Action<IReadOnlyCollection<IInteraction>> EventHandlers;
   
        public void Fire(IReadOnlyCollection<IInteraction> value)
        {
            EventHandlers?.Invoke(value);
        }

        public void Listen(Action<IReadOnlyCollection<IInteraction>> callback)
        {
            EventHandlers += callback;
        }

        public void Unlisten(Action<IReadOnlyCollection<IInteraction>> callback)
        {
            EventHandlers -= callback;
        }
 }
}