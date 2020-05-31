using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.Utility
{
    public class RuneDatabase
    {
        public readonly IReadOnlyDictionary<uint, RuneDefinition> RuneById;
        public readonly IReadOnlyList<uint> OrderedIds;

        public RuneDatabase(params RuneDefinition[] runeDefinitions)
        {
            var entryDict = new Dictionary<uint, RuneDefinition>();
            foreach (var runeDefinition in runeDefinitions) entryDict[runeDefinition.RuneId] = runeDefinition;

            RuneById = entryDict;
            OrderedIds = RuneById.Keys.OrderBy(x => x).ToList();
        }

        public RuneDatabase(string jsonDataPath)
        {
            var filePath = Path.IsPathFullyQualified(jsonDataPath)
                ? jsonDataPath
                : Path.Join(Directory.GetCurrentDirectory(), jsonDataPath);
            var jsonText = File.ReadAllText(filePath);

            RuneById = JsonConvert.DeserializeObject<Dictionary<uint, RuneDefinition>>(jsonText, JsonExtensions.DefaultSettings);
            OrderedIds = RuneById.Keys.OrderBy(x => x).ToList();
        }
    }
}