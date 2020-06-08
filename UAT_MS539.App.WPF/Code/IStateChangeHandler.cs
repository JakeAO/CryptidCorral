using UAT_MS539.Core.Code.StateMachine;

namespace UAT_MS539.Code
{
    public interface IStateChangeHandler
    {
        void OnStateChanged(IState state, Context sharedContext);
    }
}