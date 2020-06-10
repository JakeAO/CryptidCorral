using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UAT_MS539.Core.Code.Food;

namespace UAT_MS539.Core.Code.Utility
{
    public class PlayerDataUtility
    {
        private readonly string _savePath = Path.Combine(Environment.CurrentDirectory, "save.dat");
        
        private readonly JsonSerializerSettings _jsonSettings;

        public PlayerDataUtility(FoodDatabase foodDatabase, SpeciesDatabase speciesDatabase, ColorDatabase colorDatabase)
        {
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>
                {
                    new FoodConverter(foodDatabase),
                    new CryptidConverter(speciesDatabase, colorDatabase)
                }
            };
        }

        public bool TrySave(PlayerData playerData)
        {
            try
            {
                var directoryName = Path.GetDirectoryName(_savePath);
                Directory.CreateDirectory(directoryName);

                var saveText = JsonConvert.SerializeObject(playerData, _jsonSettings);
                File.WriteAllText(_savePath, saveText);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR][PlayerDataUtility.TrySave] {e.GetType().Name}: {e.Message}");
            }

            return false;
        }

        public bool TryLoad(out PlayerData playerData)
        {
            try
            {
                if (File.Exists(_savePath))
                {
                    var saveText = File.ReadAllText(_savePath);
                    if (!string.IsNullOrWhiteSpace(saveText))
                    {
                        playerData = JsonConvert.DeserializeObject<PlayerData>(saveText, _jsonSettings);
                        if (playerData != default) return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR][PlayerDataUtility.TryLoad] {e.GetType().Name}: {e.Message}");
            }

            playerData = default;
            return false;
        }

        private class FoodConverter : JsonConverter<Food.Food>
        {
            private readonly FoodDatabase _database;

            public FoodConverter(FoodDatabase database)
            {
                _database = database;
            }

            public override void WriteJson(JsonWriter writer, Food.Food value, JsonSerializer serializer)
            {
                var originalJson = JObject.FromObject(value);
                originalJson["Definition"] = new JValue(value.Definition.FoodId);
                serializer.Serialize(writer, originalJson);
            }

            public override Food.Food ReadJson(JsonReader reader, Type objectType, Food.Food existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var originalJson = JObject.Load(reader);
                var foodId = originalJson["Definition"].Value<string>();
                originalJson.Remove("Definition");
                var food = JsonConvert.DeserializeObject<Food.Food>(originalJson.ToString());
                food.Definition = _database.FoodById[foodId];
                return food;
            }
        }

        private class CryptidConverter : JsonConverter<Cryptid.Cryptid>
        {
            private readonly ColorDatabase _colorDatabase;
            private readonly SpeciesDatabase _speciesDatabase;

            public CryptidConverter(SpeciesDatabase speciesDatabase, ColorDatabase colorDatabase)
            {
                _speciesDatabase = speciesDatabase;
                _colorDatabase = colorDatabase;
            }

            public override void WriteJson(JsonWriter writer, Cryptid.Cryptid value, JsonSerializer serializer)
            {
                var originalJson = JObject.FromObject(value);
                originalJson["Species"] = new JValue(value.Species.SpeciesId);
                originalJson["Color"] = new JValue(value.Color.ColorId);
                serializer.Serialize(writer, originalJson);
            }

            public override Cryptid.Cryptid ReadJson(JsonReader reader, Type objectType, Cryptid.Cryptid existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.StartObject)
                    return null;
                
                var originalJson = JObject.Load(reader);
                var speciesId = originalJson["Species"].Value<string>();
                var colorId = originalJson["Color"].Value<string>();

                originalJson.Remove("Species");
                originalJson.Remove("Color");

                var cryptid = JsonConvert.DeserializeObject<Cryptid.Cryptid>(originalJson.ToString());
                cryptid.Species = _speciesDatabase.SpeciesById[speciesId];
                cryptid.Color = _colorDatabase.ColorById[colorId];
                return cryptid;
            }
        }
    }
}