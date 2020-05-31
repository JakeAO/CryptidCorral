using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.Utility
{
    public class ColorDatabase
    {
        public readonly IReadOnlyDictionary<string, ColorDefinition> ColorById;
        public readonly IReadOnlyList<string> OrderedIds;

        public ColorDatabase(params ColorDefinition[] colorDefinitions)
        {
            var entryDict = new Dictionary<string, ColorDefinition>(colorDefinitions.Length);
            foreach (var colorDefinition in colorDefinitions) entryDict[colorDefinition.ColorId] = colorDefinition;

            ColorById = entryDict;
            OrderedIds = ColorById.Keys.OrderBy(x => x).ToList();
        }

        public ColorDatabase(string jsonDataPath)
        {
            var filePath = Path.IsPathFullyQualified(jsonDataPath)
                ? jsonDataPath
                : Path.Join(Directory.GetCurrentDirectory(), jsonDataPath);
            var jsonText = File.ReadAllText(filePath);

            ColorById = JsonConvert.DeserializeObject<Dictionary<string, ColorDefinition>>(jsonText, JsonExtensions.DefaultSettings);
            OrderedIds = ColorById.Keys.OrderBy(x => x).ToList();
        }
    }
}