using Core.Code.StateMachine;

namespace AppWPF.Code
{
    public interface IStateChangeHandler
    {
        void OnStateChanged(IState state, Context sharedContext);
    }
}