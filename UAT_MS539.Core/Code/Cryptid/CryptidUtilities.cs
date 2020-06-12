using System;
using System.Diagnostics;
using UAT_MS539.Core.Code.Extensions;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.Cryptid
{
    public static class CryptidUtilities
    {
        public const float MAX_STAT_VAL = 575f;

        public static Cryptid CreateCryptid(
            string runicHash,
            SpeciesDatabase speciesDatabase,
            ColorDatabase colorDatabase)
        {
            Debug.Assert(runicHash.Length == 16);

            // Calculate Species
            var speciesSubstring = runicHash.Substring(0, 2);
            var speciesKey = NumberEncoder.Decode(speciesSubstring, NumberEncoder.Base24Encoding);
            var speciesPercent = speciesKey / MAX_STAT_VAL;
            var speciesId = speciesDatabase.DropCalculation.Evaluate(speciesPercent);
            if (!speciesDatabase.SpeciesById.TryGetValue(speciesId, out var species))
                throw new InvalidOperationException($"SpeciesId \"{speciesId}\" ({speciesKey}) did not exist in SpeciesDatabase.");

            // Calculate Color
            var colorSubstring = runicHash.Substring(2, 2);
            var colorKey = NumberEncoder.Decode(colorSubstring, NumberEncoder.Base24Encoding);
            var colorPercent = colorKey / MAX_STAT_VAL;
            var colorId = species.ColorFormula.Evaluate(colorPercent);
            if (!colorDatabase.ColorById.TryGetValue(colorId, out var color))
                throw new InvalidOperationException($"ColorId \"{colorId}\" ({colorKey}) did not exist in ColorDatabase");

            // Calculate Primary Stats
            var primaryStats = new uint[(int) EPrimaryStat._Count];
            for (var i = 0; i < primaryStats.Length; i++)
            {
                var startIdx = 4 + i * 2;
                var statSubstring = runicHash.Substring(startIdx, 2);
                var statKey = NumberEncoder.Decode(statSubstring, NumberEncoder.Base24Encoding);
                var statPercent = statKey / MAX_STAT_VAL;
                primaryStats[i] = species.StartingStatFormulas[(EPrimaryStat) i].Evaluate(statPercent);
            }

            var secondaryStats = new uint[(int) ESecondaryStat._Count];

            // Calculate Secondary Stat (Health)
            var healthSubstring = string.Join(string.Empty, runicHash[0], runicHash[15]);
            secondaryStats[(int) ESecondaryStat.Health] = CalculateHealth(primaryStats[(int) EPrimaryStat.Vitality], healthSubstring);

            // Calculate Secondary Stat (Renown)
            secondaryStats[(int) ESecondaryStat.Renown] = 0u;

            // Calculate Secondary Stat (Stamina)
            secondaryStats[(int) ESecondaryStat.Stamina] = 2u;

            var hiddenStats = new uint[(int) EHiddenStat._Count];

            // Calculate Hidden Stat (Morale)
            var moraleSubstring = string.Join(string.Empty, runicHash[1], runicHash[14]);
            hiddenStats[(int) EHiddenStat.Morale] = CalculateMoral(moraleSubstring);

            // Calculate Hidden Stat (Lifespan) (days)
            var lifespanSubstring = string.Join(string.Empty, runicHash[2], runicHash[13]);
            hiddenStats[(int) EHiddenStat.Lifespan] = CalculateLifespan(lifespanSubstring);

            return new Cryptid(runicHash, species, color, primaryStats, secondaryStats, hiddenStats);
        }

        public static Cryptid CreateCryptid(
            CryptidDnaSample sampleA,
            CryptidDnaSample sampleB,
            SpeciesDatabase speciesDatabase,
            ColorDatabase colorDatabase)
        {
            // Starting Stats are based on Generation + 15% of Parent Average
            const float ROLLOVER_MULTIPLIER = 0.15f;

            var hashA = sampleA.Cryptid.RunicHash;
            var hashB = sampleB.Cryptid.RunicHash;

            var newHash = string.Join(string.Empty,
                hashA[0], hashB[1],
                hashA[2], hashB[3],
                hashA[4], hashB[5],
                hashA[6], hashB[7],
                hashA[8], hashB[9],
                hashA[10], hashB[11],
                hashA[12], hashB[13],
                hashA[14], hashB[15]);

            var newCryptid = CreateCryptid(newHash, speciesDatabase, colorDatabase);

            for (var i = 0; i < (int) EPrimaryStat._Count; i++)
            {
                var sampleAverage = (sampleA.Cryptid.PrimaryStats[i] + sampleB.Cryptid.PrimaryStats[i]) / 2f;
                newCryptid.PrimaryStats[i] += (uint) Math.Round(sampleAverage * ROLLOVER_MULTIPLIER);
            }

            var averageRenown = (sampleA.Cryptid.SecondaryStats[(int) ESecondaryStat.Renown] + sampleB.Cryptid.SecondaryStats[(int) ESecondaryStat.Renown]) / 2f;
            newCryptid.SecondaryStats[(int) ESecondaryStat.Renown] = (uint) Math.Round(averageRenown * ROLLOVER_MULTIPLIER);

            var averageAge = (sampleA.Cryptid.AgeInDays + sampleB.Cryptid.AgeInDays) / 2f;
            newCryptid.HiddenStats[(int) EHiddenStat.Lifespan] += (uint) Math.Round(averageAge * ROLLOVER_MULTIPLIER);

            return newCryptid;
        }

        private static uint CalculateHealth(uint vitalityStat, string healthMultiplierHash)
        {
            const float MIN_HEALTH_MULTIPLIER = 7f;
            const float MAX_HEALTH_MULTIPLIER = 13f;

            Debug.Assert(vitalityStat > 0);
            Debug.Assert(healthMultiplierHash.Length == 2);

            var healthMultiplierKey = NumberEncoder.Decode(healthMultiplierHash, NumberEncoder.Base24Encoding);
            var healthMultiplierPercent = healthMultiplierKey / MAX_STAT_VAL;
            var healthMultiplier = healthMultiplierPercent.Remap(0f, 1f, MIN_HEALTH_MULTIPLIER, MAX_HEALTH_MULTIPLIER);
            return (uint) Math.Round(vitalityStat * healthMultiplier);
        }

        private static uint CalculateMoral(string moraleHash)
        {
            const float MIN_MORALE = 50f;
            const float MAX_MORALE = 100f;

            Debug.Assert(moraleHash.Length == 2);

            var moraleKey = NumberEncoder.Decode(moraleHash, NumberEncoder.Base24Encoding);
            var moralePercent = moraleKey / MAX_STAT_VAL;
            return (uint) Math.Round(moralePercent.Remap(0f, 1f, MIN_MORALE, MAX_MORALE));
        }

        private static uint CalculateLifespan(string lifespanHash)
        {
            const float MIN_LIFESPAN_DAYS = 5f * 7f;
            const float MAX_LIFESPAN_DAYS = 15f * 7f;

            Debug.Assert(lifespanHash.Length == 2);

            var lifespanKey = NumberEncoder.Decode(lifespanHash, NumberEncoder.Base24Encoding);
            var lifespanPercent = lifespanKey / MAX_STAT_VAL;
            return (uint) Math.Round(lifespanPercent.Remap(0f, 1f, MIN_LIFESPAN_DAYS, MAX_LIFESPAN_DAYS));
        }
    }
}