using System.Collections.Generic;
using UAT_MS539.Core.Code.StateMachine.CombatEngine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class ColiseumBattleState : IState
    {
        public string LocationLocId => "Location/Coliseum";
        public string TimeLocId => "Time/Day";

        private Context _sharedContext;
        private CombatData _combatData;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _combatData = context.Get<CombatData>();
        }

        public void PerformContent(Context context)
        {
            CombatEngine.CombatEngine combatEngine = new CombatEngine.CombatEngine(_combatData);
            IReadOnlyCollection<CombatEngine.CombatEngine.CombatEvent> combatEvents = combatEngine.ResolveCombat();

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new DisplayCombat(_combatData, combatEvents, OnCombatFinished),
            });
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void OnCombatFinished()
        {
            _sharedContext.Get<StateMachine>().ChangeState<ColiseumResultsState>();
        }
    }
}