using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.Training
{
    public class TrainingDatabase
    {
        public readonly IReadOnlyDictionary<string, TrainingRegimen> TrainingById;
        public readonly IReadOnlyList<string> OrderedIds;

        public TrainingDatabase(params TrainingRegimen[] trainingDefinitions)
        {
            var entryDict = new Dictionary<string, TrainingRegimen>(trainingDefinitions.Length);
            foreach (var trainingRegimen in trainingDefinitions) entryDict[trainingRegimen.TrainingId] = trainingRegimen;

            TrainingById = entryDict;
            OrderedIds = TrainingById.Keys.OrderBy(x => x).ToList();
        }

        public TrainingDatabase(string jsonDataPath)
        {
            var filePath = Path.IsPathFullyQualified(jsonDataPath)
                ? jsonDataPath
                : Path.Join(Directory.GetCurrentDirectory(), jsonDataPath);
            var jsonText = File.ReadAllText(filePath);

            TrainingById = JsonConvert.DeserializeObject<Dictionary<string, TrainingRegimen>>(jsonText, JsonExtensions.DefaultSettings);
            OrderedIds = TrainingById.Keys.OrderBy(x => x).ToList();
        }
    }
}