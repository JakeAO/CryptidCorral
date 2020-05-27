using System;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.Signals
{
    public class StateChanged : ISignal<IState>
    {
        private event Action<IState> EventHandlers;

        public void Fire(IState value) => EventHandlers?.Invoke(value);
        public void Listen(Action<IState> callback) => EventHandlers += callback;
        public void Unlisten(Action<IState> callback) => EventHandlers -= callback;
    }
}