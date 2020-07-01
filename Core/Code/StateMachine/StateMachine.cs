using Core.Code.StateMachine.Signals;
using Core.Code.StateMachine.States;

namespace Core.Code.StateMachine
{
    public class StateMachine
    {
        public Context SharedContext { get; }
        public IState CurrentState { get; private set; } = new NilState();
        
        public StateMachine(Context sharedContext)
        {
            SharedContext = sharedContext;
            SharedContext.Set(this);
            SharedContext.Set(new StateChanged(), false);

            ChangeState<InitializationState>();
        }

        public void ChangeState<T>() where T : IState, new()
        {
            IState nextState = new T();

            CurrentState.PerformTeardown(SharedContext, nextState);

            SharedContext.Set(nextState);

            nextState.PerformSetup(SharedContext, CurrentState);

            CurrentState = nextState;

            SharedContext.Get<StateChanged>().Fire(nextState);

            CurrentState.PerformContent(SharedContext);
        }
    }
}