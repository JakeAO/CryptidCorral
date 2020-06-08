using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.Utility
{
    public class LocEntry
    {
        public readonly string LocId;
        public readonly IReadOnlyDictionary<string, string> TextByLang;

        public LocEntry(string locId, IReadOnlyDictionary<string, string> textByLang)
        {
            LocId = locId;
            TextByLang = textByLang;
        }
    }

    public class LocDatabase
    {
        public readonly HashSet<string> KnownLangIds;
        public readonly Dictionary<string, LocEntry> LocEntryById;

        private string _currentLangId = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        public LocDatabase(params LocEntry[] locEntries)
        {
            var langHash = new HashSet<string>(10);
            var entryDict = new Dictionary<string, LocEntry>(locEntries.Length);
            foreach (var locEntry in locEntries)
            {
                entryDict[locEntry.LocId] = locEntry;
                langHash.UnionWith(locEntry.TextByLang.Keys);
            }

            LocEntryById = entryDict;
            KnownLangIds = langHash;
        }

        public LocDatabase(string jsonDataPath)
        {
            var filePath = Path.GetFullPath(jsonDataPath);
            var jsonText = File.ReadAllText(filePath);
            LocEntryById = JsonConvert.DeserializeObject<Dictionary<string, LocEntry>>(jsonText, JsonExtensions.DefaultSettings);

            var langHash = new HashSet<string>(10);
            foreach (var locEntry in LocEntryById.Values) langHash.UnionWith(locEntry.TextByLang.Keys);

            KnownLangIds = langHash;
        }

        public string CurrentLangId
        {
            get => _currentLangId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;
                if (!KnownLangIds.Contains(value))
                    return;
                _currentLangId = value;
            }
        }

        public string Localize(string locId, IReadOnlyDictionary<string, string> locParams = null)
        {
            return TryLocalize(locId, out var localizedText, locParams) ? localizedText : $"MISSING LOC: {locId}";
        }

        public bool TryLocalize(string locId, out string localizedText, IReadOnlyDictionary<string, string> locParams = null)
        {
            localizedText = null;
            if (string.IsNullOrWhiteSpace(locId))
                return false;
            if (string.IsNullOrWhiteSpace(CurrentLangId))
                return false;
            if (!LocEntryById.TryGetValue(locId, out var locEntry))
                return false;
            if (!locEntry.TextByLang.TryGetValue(CurrentLangId, out localizedText))
                return false;

            if (locParams != null)
                foreach (var locParamKvp in locParams)
                    localizedText = localizedText.Replace(locParamKvp.Key, locParamKvp.Value);

            return true;
        }
    }
}