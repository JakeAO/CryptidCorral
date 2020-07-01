using Core.Code.Cryptid;
using Core.Code.Utility;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public static class RunePatternTests
    {
        private static SpeciesDatabase _speciesDatabase = null;
        private static ColorDatabase _colorDatabase = null;
        private static RunePatternDatabase _runePatternDatabase = null;

        [SetUp]
        public static void SetUp()
        {
            _speciesDatabase = new SpeciesDatabase(@"C:\Users\otaku\Documents\SchoolProjects\MS539\UAT_MS539\UAT_MS539.Core\Source\speciesDatabase.json");
            _colorDatabase = new ColorDatabase(@"C:\Users\otaku\Documents\SchoolProjects\MS539\UAT_MS539\UAT_MS539.Core\Source\colorDatabase.json");
            _runePatternDatabase = new RunePatternDatabase();
        }

        [Test]
        public static void TestCryptidsFromPatterns()
        {
            foreach (RunePattern runePattern in _runePatternDatabase.KnownRunePatterns)
            {
                string runeHash = runePattern.RunicHash;
                Assert.IsNotNull(runeHash);
                Assert.IsNotEmpty(runeHash);

                Cryptid newCryptid = CryptidUtilities.CreateCryptid(runeHash, _speciesDatabase, _colorDatabase);
                Assert.IsNotNull(newCryptid);
                Assert.IsNotNull(newCryptid.Species);
                Assert.IsNotNull(newCryptid.Color);
            }
        }
    }
}