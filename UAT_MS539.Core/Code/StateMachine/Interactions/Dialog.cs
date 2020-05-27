using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class Dialog : IInteraction
    {
        public readonly string LocId;
        public readonly IReadOnlyDictionary<string, string> LocParams;

        public Dialog(string locId, params KeyValuePair<string, string>[] locParams)
        {
            LocId = locId;
            LocParams = new Dictionary<string, string>(locParams);
        }
    }
}