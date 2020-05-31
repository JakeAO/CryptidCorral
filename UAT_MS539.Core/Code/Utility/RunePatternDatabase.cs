using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UAT_MS539.Core.Code.Utility
{
    public class RunePatternDatabase
    {
        public readonly IReadOnlyList<RunePattern> KnownRunePatterns;
        public readonly IReadOnlyList<string> OrderedIds;

        public RunePatternDatabase()
        {
            const string searchPattern = "*.exe";

            var runePatterns = new List<RunePattern>(100);

            var baseDirectory64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var exeFiles64 = Directory.GetFiles(baseDirectory64, searchPattern, new EnumerationOptions
            {
                IgnoreInaccessible = true,
                MatchType = MatchType.Simple,
                RecurseSubdirectories = true
            });
            CalculateRunePatterns(exeFiles64, runePatterns);

            // string baseDirectory86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            // string[] exeFiles86 = Directory.GetFiles(baseDirectory86, searchPattern,  new EnumerationOptions()
            // {
            //     IgnoreInaccessible = true,
            //     MatchType = MatchType.Simple,
            //     RecurseSubdirectories = true
            // });
            // CalculateRunePatterns(exeFiles86, runePatterns);

            KnownRunePatterns = runePatterns;
            OrderedIds = runePatterns.Select(x => x.RunicHash).OrderBy(x => x).ToList();
        }

        private void CalculateRunePatterns(string[] filePaths, List<RunePattern> patternList)
        {
            foreach (var filePath in filePaths)
                try
                {
                    var fi = new FileInfo(filePath);
                    if (!fi.Exists)
                        continue;

                    var md5Precursor = new StringBuilder();
                    md5Precursor.AppendLine(fi.Name);
                    md5Precursor.AppendLine(fi.CreationTimeUtc.ToLongDateString());
                    md5Precursor.AppendLine(fi.CreationTimeUtc.ToLongTimeString());
                    md5Precursor.AppendLine(fi.Length.ToString());
                    var precursorBytes = Encoding.Unicode.GetBytes(md5Precursor.ToString());

                    using var cryptoProvider = new MD5CryptoServiceProvider();
                    var md5HashBytes = cryptoProvider.ComputeHash(precursorBytes);

                    var md5Hash = string.Concat(md5HashBytes.Select(x => x.ToString("X2"))).ToUpper();
                    var paddedMd5Hash = md5Hash + md5Hash.Substring(0, 4); // Pad out to 36 characters so we can convert 2 base-16 values into 1 base-24 value.

                    var runicPrecursor = new StringBuilder();
                    for (var i = 0; i < paddedMd5Hash.Length; i += 2)
                    {
                        var twoDigitBase16 = paddedMd5Hash.Substring(i, 2);
                        var base16Value = NumberEncoder.Decode(twoDigitBase16, NumberEncoder.Base16Encoding);
                        base16Value %= 24; // Wrap around the 1-digit max for base-24
                        var oneDigitBase24 = NumberEncoder.Encode(base16Value, NumberEncoder.Base24Encoding);
                        runicPrecursor.Append(oneDigitBase24);
                    }

                    var runicHash = runicPrecursor.ToString();

                    patternList.Add(new RunePattern(
                        fi.FullName,
                        paddedMd5Hash,
                        runicHash));
                }
                catch
                {
                    // Ignore
                }
        }

        public readonly struct RunePattern
        {
            public readonly string FilePath;
            public readonly string Md5;
            public readonly string RunicHash;

            public RunePattern(string filePath, string md5, string runicHash)
            {
                FilePath = filePath;
                Md5 = md5;
                RunicHash = runicHash;
            }
        }
    }
}