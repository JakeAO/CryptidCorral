using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.Utility
{
    public class PatternDatabase
    {
        public readonly IReadOnlyDictionary<string, PatternDefinition> PatternById;
        public readonly IReadOnlyList<string> OrderedIds;

        public PatternDatabase(params PatternDefinition[] patternDefinitions)
        {
            var entryDict = new Dictionary<string, PatternDefinition>(patternDefinitions.Length);
            foreach (var patternDefinition in patternDefinitions) entryDict[patternDefinition.PatternId] = patternDefinition;

            PatternById = entryDict;
            OrderedIds = PatternById.Keys.OrderBy(x => x).ToList();
        }

        public PatternDatabase(string jsonDataPath)
        {
            var filePath = Path.IsPathFullyQualified(jsonDataPath)
                ? jsonDataPath
                : Path.Join(Directory.GetCurrentDirectory(), jsonDataPath);
            var jsonText = File.ReadAllText(filePath);

            PatternById = JsonConvert.DeserializeObject<Dictionary<string, PatternDefinition>>(jsonText, JsonExtensions.DefaultSettings);
            OrderedIds = PatternById.Keys.OrderBy(x => x).ToList();
        }
    }
}