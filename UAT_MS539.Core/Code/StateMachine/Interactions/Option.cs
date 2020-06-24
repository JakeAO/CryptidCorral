using System;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class Option : IInteraction
    {
        public readonly string LocId;
        public readonly Action ActionHandler;
        public readonly string TooltipId;

        public Option(string locId, Action actionHandler, string tooltipId = null)
        {
            LocId = locId;
            ActionHandler = actionHandler;
            TooltipId = tooltipId;
        }
    }
}