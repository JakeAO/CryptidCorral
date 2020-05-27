using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UAT_MS539.Core.Code.Utility
{
    public class RunePatternDatabase
    {
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

        public readonly IReadOnlyList<RunePattern> KnownRunePatterns;
        public readonly IReadOnlyList<string> OrderedIds;

        public RunePatternDatabase()
        {
            const string searchPattern = "*.exe";

            List<RunePattern> runePatterns = new List<RunePattern>(100);

            string baseDirectory64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string[] exeFiles64 = Directory.GetFiles(baseDirectory64, searchPattern, new EnumerationOptions()
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
            foreach (string filePath in filePaths)
            {
                try
                {
                    FileInfo fi = new FileInfo(filePath);
                    if (!fi.Exists)
                        continue;

                    StringBuilder md5Precursor = new StringBuilder();
                    md5Precursor.AppendLine(fi.Name);
                    md5Precursor.AppendLine(fi.CreationTimeUtc.ToLongDateString());
                    md5Precursor.AppendLine(fi.CreationTimeUtc.ToLongTimeString());
                    md5Precursor.AppendLine(fi.Length.ToString());
                    byte[] precursorBytes = Encoding.Unicode.GetBytes(md5Precursor.ToString());

                    using var cryptoProvider = new MD5CryptoServiceProvider();
                    byte[] md5HashBytes = cryptoProvider.ComputeHash(precursorBytes);

                    string md5Hash = string.Concat(md5HashBytes.Select(x => x.ToString("X2"))).ToUpper();
                    string paddedMd5Hash = md5Hash + md5Hash.Substring(0, 4); // Pad out to 36 characters so we can convert 2 base-16 values into 1 base-24 value.

                    StringBuilder runicPrecursor = new StringBuilder();
                    for (int i = 0; i < paddedMd5Hash.Length; i += 2)
                    {
                        string twoDigitBase16 = paddedMd5Hash.Substring(i, 2);
                        long base16Value = NumberEncoder.Decode(twoDigitBase16, NumberEncoder.Base16Encoding);
                        base16Value %= 24; // Wrap around the 1-digit max for base-24
                        string oneDigitBase24 = NumberEncoder.Encode(base16Value, NumberEncoder.Base24Encoding);
                        runicPrecursor.Append(oneDigitBase24);
                    }

                    string runicHash = runicPrecursor.ToString();

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
        }
    }
}