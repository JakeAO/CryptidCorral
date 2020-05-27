using System.Diagnostics;

namespace UAT_MS539.Core.Code.Cryptid
{
    public class Cryptid
    {
        public SpeciesDefinition Species;
        public PatternDefinition Pattern;
        public ColorDefinition Color;

        public uint[] PrimaryStats;
        public uint[] PrimaryStatExp;
        public uint[] SecondaryStats;

        public Cryptid(SpeciesDefinition species, PatternDefinition pattern, ColorDefinition color,
            uint[] primaryStats, uint[] secondaryStats)
        {
            Debug.Assert(species != null);
            Debug.Assert(primaryStats.Length == (int) EPrimaryStat._Count);
            Debug.Assert(secondaryStats.Length == (int) ESecondaryStat._Count);

            Species = species;
            Pattern = pattern;
            Color = color;
            PrimaryStats = primaryStats;
            PrimaryStatExp = new uint[(int) EPrimaryStat._Count];
            SecondaryStats = secondaryStats;
        }
    }
}