using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Code.Utility
{
    public class RunePatternDatabase
    {
        private const string SEARCH_PATTERN = "*.exe";

        public readonly IReadOnlyList<RunePattern> KnownRunePatterns;
        public readonly IReadOnlyList<string> OrderedIds;

        public RunePatternDatabase()
        {
            var runePatterns = new List<RunePattern>(1000);

            Task.WaitAll(
                Task.Run(() => FindExeRecursive(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)), runePatterns)),
                Task.Run(() => FindExeRecursive(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)), runePatterns)));

            KnownRunePatterns = runePatterns;
            OrderedIds = runePatterns.Select(x => x.RunicHash).OrderBy(x => x).ToList();
        }

        private void FindExeRecursive(DirectoryInfo directoryInfo, List<RunePattern> runePatterns)
        {
            try
            {
                if (directoryInfo.Exists)
                {
                    foreach (FileInfo fi in directoryInfo.EnumerateFiles(SEARCH_PATTERN))
                    {
                        if (TryCalculateRunePattern(fi, out RunePattern runePattern))
                        {
                            // Just grab the first .exe in each folder, otherwise we get multiple-thousands of runePatterns in the end.
                            runePatterns.Add(runePattern);
                            break;
                        }
                    }

                    foreach (DirectoryInfo di in directoryInfo.EnumerateDirectories())
                    {
                        FindExeRecursive(di, runePatterns);
                    }
                }
            }
            catch
            {
                // Ignore
                // Near-impossible to prevent IO errors, especially UnauthorizedAccessExceptions, and it doesn't really matter for the purposes of this method.
            }
        }

        private bool TryCalculateRunePattern(FileInfo fileInfo, out RunePattern runePattern)
        {
            runePattern = default;
            try
            {
                if (!fileInfo.Exists)
                    return false;

                var md5Precursor = new StringBuilder();
                md5Precursor.AppendLine(fileInfo.Name);
                md5Precursor.AppendLine(fileInfo.CreationTimeUtc.ToLongDateString());
                md5Precursor.AppendLine(fileInfo.CreationTimeUtc.ToLongTimeString());
                md5Precursor.AppendLine(fileInfo.Length.ToString());
                var precursorBytes = Encoding.Unicode.GetBytes(md5Precursor.ToString());

                using var cryptoProvider = new MD5CryptoServiceProvider();
                var md5HashBytes = cryptoProvider.ComputeHash(precursorBytes);

                var md5Hash = string.Concat(md5HashBytes.Select(x => x.ToString("X2"))).ToUpper();

                var runicPrecursor = new StringBuilder();
                for (var i = 0; i < md5Hash.Length; i += 2)
                {
                    var twoDigitBase16 = md5Hash.Substring(i, 2);
                    var base16Value = NumberEncoder.Decode(twoDigitBase16, NumberEncoder.Base16Encoding);
                    base16Value %= 24; // Wrap around the 1-digit max for base-24
                    var oneDigitBase24 = NumberEncoder.Encode(base16Value, NumberEncoder.Base24Encoding);
                    runicPrecursor.Append(oneDigitBase24);
                }

                var runicHash = runicPrecursor.ToString();

                runePattern = new RunePattern(
                    fileInfo.FullName,
                    md5Hash,
                    runicHash);
                return true;
            }
            catch
            {
                // Ignore
                return false;
            }
        }
    }
}