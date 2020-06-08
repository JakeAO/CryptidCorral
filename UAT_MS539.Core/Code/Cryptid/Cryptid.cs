using System.Diagnostics;

namespace UAT_MS539.Core.Code.Cryptid
{
    public class Cryptid
    {
        public string RunicHash;
        public SpeciesDefinition Species;
        public ColorDefinition Color;
        
        public uint[] PrimaryStats;
        public uint[] PrimaryStatExp;
        public uint[] SecondaryStats;
        public uint[] HiddenStats;
        
        public uint AgeInDays;
        
        public uint CurrentHealth;
        public uint CurrentMorale;
        public uint CurrentStamina;

        public Cryptid(string runicHash, SpeciesDefinition species, ColorDefinition color,
            uint[] primaryStats, uint[] secondaryStats, uint[] hiddenStats)
        {
            Debug.Assert(runicHash.Length == 16);
            Debug.Assert(primaryStats.Length == (int) EPrimaryStat._Count);
            Debug.Assert(secondaryStats.Length == (int) ESecondaryStat._Count);
            Debug.Assert(hiddenStats.Length == (int) EHiddenStat._Count);

            RunicHash = runicHash;
            Species = species;
            Color = color;
            PrimaryStats = primaryStats;
            PrimaryStatExp = new uint[(int) EPrimaryStat._Count];
            SecondaryStats = secondaryStats;
            HiddenStats = hiddenStats;

            AgeInDays = 0;
            CurrentHealth = secondaryStats[(int) ESecondaryStat.Health];
            CurrentStamina = secondaryStats[(int) ESecondaryStat.Stamina];
            CurrentMorale = hiddenStats[(int) EHiddenStat.Morale];
        }
    }
}