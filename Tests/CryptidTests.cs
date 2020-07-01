using System;
using System.Collections.Generic;
using System.Linq;
using Core.Code;
using Core.Code.Cryptid;
using Core.Code.Utility;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public static class CryptidTests
    {
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        public static void TestRandomRunicPatterns(int testCount)
        {
            Random random = new Random();
            SpeciesDatabase speciesDatabase = new SpeciesDatabase(@"C:\Users\otaku\Documents\SchoolProjects\MS539\UAT_MS539\UAT_MS539.Core\Source\speciesDatabase.json");
            ColorDatabase colorDatabase = new ColorDatabase(@"C:\Users\otaku\Documents\SchoolProjects\MS539\UAT_MS539\UAT_MS539.Core\Source\colorDatabase.json");

            IEnumerable<string> hashGenBase = Enumerable.Repeat(NumberEncoder.Base24Encoding, 16);
            for (int test = 0; test < testCount; test++)
            {
                string randomHash = new string(hashGenBase
                    .Select(x => x[random.Next(x.Length)])
                    .ToArray());
                Assert.IsNotNull(randomHash);
                Assert.IsNotEmpty(randomHash);
                Assert.AreEqual(randomHash.Length, 16);
                
                Cryptid newCryptid = CryptidUtilities.CreateCryptid(randomHash, speciesDatabase, colorDatabase);
                Assert.IsNotNull(newCryptid);
                Assert.IsNotNull(newCryptid.Species);
                Assert.IsNotNull(newCryptid.Color);
            }
        }
    }
}