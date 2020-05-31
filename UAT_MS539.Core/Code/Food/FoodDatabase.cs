using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Extensions;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.Food
{
    public class FoodDatabase
    {
        public readonly IReadOnlyDictionary<string, FoodDefinition> FoodById;
        public readonly DropCalculation<string> FoodSpawnRate;
        public readonly IReadOnlyList<string> OrderedIds;

        public FoodDatabase(params FoodDefinition[] foodDefinitions)
        {
            var entryDict = new Dictionary<string, FoodDefinition>(foodDefinitions.Length);
            foreach (var foodDefinition in foodDefinitions) entryDict[foodDefinition.FoodId] = foodDefinition;

            FoodById = entryDict;
            OrderedIds = FoodById.Keys.OrderBy(x => x).ToList();
            FoodSpawnRate = new DropCalculation<string>(FoodById.Select(x => new DropCalculation<string>.Point(x.Key, x.Value.SpawnRate)).ToArray());
        }

        public FoodDatabase(string jsonDataPath)
        {
            var filePath = Path.IsPathFullyQualified(jsonDataPath)
                ? jsonDataPath
                : Path.Join(Directory.GetCurrentDirectory(), jsonDataPath);
            var jsonText = File.ReadAllText(filePath);

            FoodById = JsonConvert.DeserializeObject<Dictionary<string, FoodDefinition>>(jsonText, JsonExtensions.DefaultSettings);
            OrderedIds = FoodById.Keys.OrderBy(x => x).ToList();
            FoodSpawnRate = new DropCalculation<string>(FoodById.Select(x => new DropCalculation<string>.Point(x.Key, x.Value.SpawnRate)).ToArray());
        }
    }
}