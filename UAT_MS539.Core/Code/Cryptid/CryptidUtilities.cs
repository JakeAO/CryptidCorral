using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UAT_MS539.Core.Code.Extensions;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.Cryptid
{
    public static class CryptidUtilities
    {
        private const float MAX_VAL = 575f;
     
        public static CryptidDnaSample CreateDnaSample(
            string runicHash,
            SpeciesDatabase speciesDatabase,
            PatternDatabase patternDatabase,
            ColorDatabase colorDatabase)
        {
            Debug.Assert(runicHash.Length == 20);

            long speciesKey = NumberEncoder.Decode(runicHash.Substring(0, 2), NumberEncoder.Base24Encoding);
            string speciesId = speciesDatabase.OrderedIds[(int) (speciesKey % speciesDatabase.OrderedIds.Count)];

            long patternKey = NumberEncoder.Decode(runicHash.Substring(2, 2), NumberEncoder.Base24Encoding);
            string patternId = patternDatabase.OrderedIds[(int) (patternKey % patternDatabase.OrderedIds.Count)];

            long colorKey = NumberEncoder.Decode(runicHash.Substring(4, 2), NumberEncoder.Base24Encoding);
            string colorId = colorDatabase.OrderedIds[(int) (colorKey % colorDatabase.OrderedIds.Count)];

            uint[] primaryStats = new uint[(int) EPrimaryStat._Count];
            for (int i = 0; i < primaryStats.Length; i++)
            {
                int startIdx = 6 + i * 2;
                long statKey = NumberEncoder.Decode(runicHash.Substring(startIdx, 2), NumberEncoder.Base24Encoding);
                primaryStats[i] = (uint) statKey;
            }

            uint renownKey = (uint) NumberEncoder.Decode(runicHash.Substring(18, 2), NumberEncoder.Base24Encoding);

            return new CryptidDnaSample(speciesId, patternId, colorId, primaryStats, renownKey);
        }

        public static CryptidDnaSample CreateDnaSample(Cryptid cryptid)
        {
            return new CryptidDnaSample(
                cryptid.Species.SpeciesId,
                cryptid.Pattern.PatternId,
                cryptid.Color.ColorId,
                cryptid.PrimaryStats.ToArray(),
                cryptid.SecondaryStats[(int) ESecondaryStat.Renown]);
        }

        public static string CreateRunicHash(
            CryptidDnaSample dnaSample,
            SpeciesDatabase speciesDatabase,
            PatternDatabase patternDatabase,
            ColorDatabase colorDatabase)
        {
            StringBuilder sb = new StringBuilder(20);

            string speciesKey = NumberEncoder.Encode(speciesDatabase.OrderedIds.ToList().IndexOf(dnaSample.SpeciesId), NumberEncoder.Base24Encoding);
            sb.Append(speciesKey.PadLeft(2, '0'));
            string patternKey = NumberEncoder.Encode(patternDatabase.OrderedIds.ToList().IndexOf(dnaSample.PatternId), NumberEncoder.Base24Encoding);
            sb.Append(patternKey.PadLeft(2, '0'));
            string colorKey = NumberEncoder.Encode(colorDatabase.OrderedIds.ToList().IndexOf(dnaSample.ColorId), NumberEncoder.Base24Encoding);
            sb.Append(colorKey.PadLeft(2, '0'));

            for (int i = 0; i < dnaSample.PrimaryStats.Length; i++)
            {
                string statKey = NumberEncoder.Encode(dnaSample.PrimaryStats[i], NumberEncoder.Base24Encoding);
                sb.Append(statKey.PadLeft(2, '0'));
            }

            string renownKey = NumberEncoder.Encode(dnaSample.Renown, NumberEncoder.Base24Encoding);
            sb.Append(renownKey.PadLeft(2, '0'));

            Debug.Assert(sb.Length == 20);

            return sb.ToString();
        }

        public static Cryptid CreateCryptid(
            string runicHash,
            SpeciesDatabase speciesDatabase,
            PatternDatabase patternDatabase,
            ColorDatabase colorDatabase)
        {
            Debug.Assert(runicHash.Length == 18);

            // Calculate Species
            string speciesSubstring = runicHash.Substring(0, 2);
            long speciesKey = NumberEncoder.Decode(speciesSubstring, NumberEncoder.Base24Encoding);
            float speciesPercent = speciesKey / MAX_VAL;
            string speciesId = speciesDatabase.DropCalculation.Evaluate(speciesPercent);
            if (!speciesDatabase.SpeciesById.TryGetValue(speciesId, out SpeciesDefinition species))
                throw new InvalidOperationException($"SpeciesId \"{speciesId}\" ({speciesKey}) did not exist in SpeciesDatabase.");

            // Calculate Pattern
            string patternSubstring = runicHash.Substring(2, 2);
            long patternKey = NumberEncoder.Decode(patternSubstring, NumberEncoder.Base24Encoding);
            float patternPercent = patternKey / MAX_VAL;
            string patternId = species.PatternFormula.Evaluate(patternPercent);
            if (!patternDatabase.PatternById.TryGetValue(patternId, out PatternDefinition pattern))
                throw new InvalidOperationException($"PatternId \"{patternId}\" ({patternKey}) did not exist in PatternDatabase.");

            // Calculate Color
            string colorSubstring = runicHash.Substring(4, 2);
            long colorKey = NumberEncoder.Decode(colorSubstring, NumberEncoder.Base24Encoding);
            float colorPercent = colorKey / MAX_VAL;
            string colorId = species.ColorFormula.Evaluate(colorPercent);
            if (!colorDatabase.ColorById.TryGetValue(colorId, out ColorDefinition color))
                throw new InvalidOperationException($"ColorId \"{colorId}\" ({colorKey}) did not exist in ColorDatabase");

            // Calculate Primary Stats
            uint[] primaryStats = new uint[(int) EPrimaryStat._Count];
            for (int i = 0; i < primaryStats.Length; i++)
            {
                int startIdx = 6 + i * 2;
                string statSubstring = runicHash.Substring(startIdx, 2);
                long statKey = NumberEncoder.Decode(statSubstring, NumberEncoder.Base24Encoding);
                float statPercent = statKey / MAX_VAL;
                primaryStats[i] = species.StartingStatFormulas[(EPrimaryStat) i].Evaluate(statPercent);
            }

            uint[] secondaryStats = new uint[(int) ESecondaryStat._Count];

            // Calculate Secondary Stat (Health)
            string healthSubstring = string.Join(string.Empty, runicHash[0], runicHash[^1]);
            secondaryStats[(int) ESecondaryStat.Health] = CalculateHealth(primaryStats[(int) EPrimaryStat.Vitality], healthSubstring);

            // Calculate Secondary Stat (Renown)
            secondaryStats[(int) ESecondaryStat.Renown] = 0u;

            // Calculate Secondary Stat (Morale)
            string moraleSubstring = string.Join(string.Empty, runicHash[1], runicHash[^2]);
            secondaryStats[(int) ESecondaryStat.Morale] = CalculateMoral(moraleSubstring);

            // Calculate Secondary Stat (Lifespan)
            string lifespanSubstring = string.Join(string.Empty, runicHash[2], runicHash[^3]);
            secondaryStats[(int) ESecondaryStat.Lifespan] = CalculateLifespan(lifespanSubstring);

            return new Cryptid(species, pattern, color, primaryStats, secondaryStats);
        }

        public static Cryptid CreateCryptid(
            CryptidDnaSample sampleA,
            CryptidDnaSample sampleB,
            SpeciesDatabase speciesDatabase,
            PatternDatabase patternDatabase,
            ColorDatabase colorDatabase)
        {
            // Starting Stats are based on Generation + 20% of Parent Average
            const float ROLLOVER_MULTIPLIER = 0.2f;

            string hashA = CreateRunicHash(sampleA, speciesDatabase, patternDatabase, colorDatabase);
            string hashB = CreateRunicHash(sampleB, speciesDatabase, patternDatabase, colorDatabase);

            string newHash = string.Join(string.Empty,
                hashA[0], hashB[1],
                hashA[2], hashB[3],
                hashA[4], hashB[5],
                hashA[6], hashB[7],
                hashA[8], hashB[9],
                hashA[10], hashB[11],
                hashA[12], hashB[13],
                hashA[14], hashB[15],
                hashA[16], hashB[17]);

            Cryptid newCryptid = CreateCryptid(newHash, speciesDatabase, patternDatabase, colorDatabase);

            for (int i = 0; i < (int) EPrimaryStat._Count; i++)
            {
                float sampleAverage = (sampleA.PrimaryStats[i] + sampleB.PrimaryStats[i]) / 2f;
                newCryptid.PrimaryStats[i] += (uint) Math.Round(sampleAverage * ROLLOVER_MULTIPLIER);
            }

            newCryptid.SecondaryStats[(int) ESecondaryStat.Renown] = (uint) Math.Round((sampleA.Renown + sampleB.Renown) / 2f * ROLLOVER_MULTIPLIER);

            return newCryptid;
        }

        private static uint CalculateHealth(uint vitalityStat, string healthMultiplierHash)
        {
            const float MIN_HEALTH_MULTIPLIER = 7f;
            const float MAX_HEALTH_MULTIPLIER = 13f;

            Debug.Assert(vitalityStat > 0);
            Debug.Assert(healthMultiplierHash.Length == 2);

            long healthMultiplierKey = NumberEncoder.Decode(healthMultiplierHash, NumberEncoder.Base24Encoding);
            float healthMultiplierPercent = healthMultiplierKey / MAX_VAL;
            float healthMultiplier = healthMultiplierPercent.Remap(0f, 1f, MIN_HEALTH_MULTIPLIER, MAX_HEALTH_MULTIPLIER);
            return (uint) Math.Round(vitalityStat * healthMultiplier);
        }

        private static uint CalculateMoral(string moraleHash)
        {
            const float MIN_MORALE = 50f;
            const float MAX_MORALE = 100f;

            Debug.Assert(moraleHash.Length == 2);

            long moraleKey = NumberEncoder.Decode(moraleHash, NumberEncoder.Base24Encoding);
            float moralePercent = moraleKey / MAX_VAL;
            return (uint) Math.Round(moralePercent.Remap(0f, 1f, MIN_MORALE, MAX_MORALE));
        }

        private static uint CalculateLifespan(string lifespanHash)
        {
            const float MIN_LIFESPAN_WEEKS = 5f;
            const float MAX_LIFESPAN_WEEKS = 15f;

            Debug.Assert(lifespanHash.Length == 2);

            long lifespanKey = NumberEncoder.Decode(lifespanHash, NumberEncoder.Base24Encoding);
            float lifespanPercent = lifespanKey / MAX_VAL;
            return (uint) Math.Round(lifespanPercent.Remap(0f, 1f, MIN_LIFESPAN_WEEKS, MAX_LIFESPAN_WEEKS));
        }
    }
}