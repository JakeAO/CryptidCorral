﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Code.Cryptid;
using Core.Code.Extensions;
using Newtonsoft.Json;

namespace Core.Code.Utility
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
            DropCalculation = new DropCalculation<string>(SpeciesById.Values.Select(x => new DropCalculation<string>.Point(x.SpeciesId, x.SpawnRate)).ToArray());
        }

        public SpeciesDatabase(string speciesDataPath)
        {
            var speciesFilePath = Path.GetFullPath(speciesDataPath);
            var speciesJsonText = File.ReadAllText(speciesFilePath);

            SpeciesById = JsonConvert.DeserializeObject<Dictionary<string, SpeciesDefinition>>(speciesJsonText, JsonExtensions.DefaultSettings);
            OrderedIds = SpeciesById.Keys.OrderBy(x => x).ToList();
            DropCalculation = new DropCalculation<string>(SpeciesById.Values.Select(x => new DropCalculation<string>.Point(x.SpeciesId, x.SpawnRate)).ToArray());
        }
    }
}