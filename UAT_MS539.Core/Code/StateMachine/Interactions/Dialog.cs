using System.Collections.Generic;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class Dialog : IInteraction
    {
        public readonly string LocId;
        public readonly IReadOnlyDictionary<string, string> LocParams;

        public Dialog(string locId, params KeyValuePair<string, string>[] locParams)
        {
            Dictionary<string, string> newDict = new Dictionary<string, string>();
            foreach (var keyValuePair in locParams)
            {
                newDict[keyValuePair.Key] = keyValuePair.Value;
            }

            LocId = locId;
            LocParams = newDict;
        }
    }
}