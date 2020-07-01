using Newtonsoft.Json;

namespace Core.Code.Extensions
{
    public static class JsonExtensions
    {
        public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };
    }
}