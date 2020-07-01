using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Code.Extensions;
using Core.Code.Utility;
using Newtonsoft.Json;

namespace Core.Code.Training
{
    public class TrainingDatabase
    {
        public readonly IReadOnlyDictionary<string, TrainingRegimen> TrainingById;
        public readonly DropCalculation<string> TrainingSpawnRate;
        public readonly IReadOnlyList<string> OrderedIds;

        public TrainingDatabase(params TrainingRegimen[] trainingDefinitions)
        {
            var entryDict = new Dictionary<string, TrainingRegimen>(trainingDefinitions.Length);
            foreach (var trainingRegimen in trainingDefinitions) entryDict[trainingRegimen.TrainingId] = trainingRegimen;

            TrainingById = entryDict;
            OrderedIds = TrainingById.Keys.OrderBy(x => x).ToList();
            TrainingSpawnRate = new DropCalculation<string>(TrainingById.Select(x => new DropCalculation<string>.Point(x.Key, x.Value.SpawnRate)).ToArray());
        }

        public TrainingDatabase(string jsonDataPath)
        {
            var filePath = Path.GetFullPath(jsonDataPath);
            var jsonText = File.ReadAllText(filePath);

            TrainingById = JsonConvert.DeserializeObject<Dictionary<string, TrainingRegimen>>(jsonText, JsonExtensions.DefaultSettings);
            OrderedIds = TrainingById.Keys.OrderBy(x => x).ToList();
            TrainingSpawnRate = new DropCalculation<string>(TrainingById.Select(x => new DropCalculation<string>.Point(x.Key, x.Value.SpawnRate)).ToArray());
        }
    }
}