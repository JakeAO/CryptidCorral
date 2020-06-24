using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.StateMachine.CombatEngine;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class DisplayCombat : IInteraction
    {
        public readonly CombatData CombatData = null;
        public readonly IReadOnlyCollection<CombatEngine.CombatEngine.CombatEvent> CombatEvents = null;
        public readonly Action SkipCombatHandler = null;

        public DisplayCombat(CombatData combatData, IReadOnlyCollection<CombatEngine.CombatEngine.CombatEvent> combatEvents, Action skipCombatHandler)
        {
            CombatData = combatData;
            CombatEvents = combatEvents;
            SkipCombatHandler = skipCombatHandler;
        }
    }
}