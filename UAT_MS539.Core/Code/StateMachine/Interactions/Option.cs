using System;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class Option : IInteraction
    {
        public readonly string LocId;
        public readonly Action ActionHandler;

        public Option(string locId, Action actionHandler)
        {
            LocId = locId;
            ActionHandler = actionHandler;
        }
    }
}