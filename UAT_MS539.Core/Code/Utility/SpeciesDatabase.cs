using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.Utility
{
    public class SpeciesDatabase
    {
        public readonly IReadOnlyDictionary<string, SpeciesDefinition> SpeciesById;
        public readonly IReadOnlyList<string> OrderedIds;
        public readonly DropCalculation<string> DropCalculation;
        
        public SpeciesDatabase(params SpeciesDefinition[] speciesDefinitions)
        {
            var entryDict = new Dictionary<string, SpeciesDefinition>(speciesDefinitions.Length);
            foreach (var speciesDefinition in speciesDefinitions) entryDict[speciesDefinition.SpeciesId] = speciesDefinition;

            SpeciesById = entryDict;
            OrderedIds = SpeciesById.Keys.OrderBy(x => x).ToList();
        }

        public SpeciesDatabase(string speciesDataPath, string dropDataPath)
        {
            var speciesFilePath = Path.IsPathFullyQualified(speciesDataPath)
                ? speciesDataPath
                : Path.Join(Directory.GetCurrentDirectory(), speciesDataPath);
            var speciesJsonText = File.ReadAllText(speciesFilePath);

            SpeciesById = JsonConvert.DeserializeObject<Dictionary<string, SpeciesDefinition>>(speciesJsonText, JsonExtensions.DefaultSettings);
            OrderedIds = SpeciesById.Keys.OrderBy(x => x).ToList();

            var dropFilePath = Path.IsPathFullyQualified(dropDataPath)
                ? dropDataPath
                : Path.Join(Directory.GetCurrentDirectory(), dropDataPath);
            var dropJsonText = File.ReadAllText(dropFilePath);

            DropCalculation = JsonConvert.DeserializeObject<DropCalculation<string>>(dropJsonText, JsonExtensions.DefaultSettings);
        }
    }
}