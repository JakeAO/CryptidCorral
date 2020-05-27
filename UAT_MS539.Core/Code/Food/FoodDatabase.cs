using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.Food
{
    public class FoodDatabase
    {
        public readonly IReadOnlyDictionary<string, FoodDefinition> FoodById;
        public readonly IReadOnlyList<string> OrderedIds;

        public FoodDatabase(params FoodDefinition[] foodDefinitions)
        {
            Dictionary<string, FoodDefinition> entryDict = new Dictionary<string, FoodDefinition>(foodDefinitions.Length);
            foreach (FoodDefinition foodDefinition in foodDefinitions)
            {
                entryDict[foodDefinition.FoodId] = foodDefinition;
            }

            FoodById = entryDict;
            OrderedIds = FoodById.Keys.OrderBy(x => x).ToList();
        }

        public FoodDatabase(string jsonDataPath)
        {
            string filePath = Path.IsPathFullyQualified(jsonDataPath)
                ? jsonDataPath
                : Path.Join(Directory.GetCurrentDirectory(), jsonDataPath);
            string jsonText = File.ReadAllText(filePath);
            
            FoodById = JsonConvert.DeserializeObject<Dictionary<string, FoodDefinition>>(jsonText, JsonExtensions.DefaultSettings);
            OrderedIds = FoodById.Keys.OrderBy(x => x).ToList();
        }
    }
}