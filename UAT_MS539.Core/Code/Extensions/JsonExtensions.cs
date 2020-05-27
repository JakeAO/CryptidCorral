using Newtonsoft.Json;

namespace UAT_MS539.Core.Code.Extensions
{
    public static class JsonExtensions
    {
        public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };
    }
}