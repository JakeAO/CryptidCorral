using System.Diagnostics;

namespace Core.Code.Cryptid
{
    public class Cryptid
    {
        public string RunicHash;
        public SpeciesDefinition Species;
        public ColorDefinition Color;
        
        public uint[] PrimaryStats;
        public uint[] PrimaryStatExp;
        public uint[] HiddenStats;
        
        public uint AgeInDays;
        
        public uint CurrentHealth;
        public uint CurrentMorale;
        public uint CurrentStamina;
        public uint CurrentRenown;

        public uint MaxHealth => PrimaryStats[(int) EPrimaryStat.Vitality] * HiddenStats[(int) EHiddenStat.HealthMultiplier];
        public uint MaxStamina => 2 + PrimaryStats[(int) EPrimaryStat.Vitality] / 100u;
        
        public Cryptid(string runicHash, SpeciesDefinition species, ColorDefinition color,
            uint[] primaryStats, uint[] hiddenStats)
        {
            Debug.Assert(runicHash.Length == 16);
            Debug.Assert(primaryStats.Length == (int) EPrimaryStat._Count);
            Debug.Assert(hiddenStats.Length == (int) EHiddenStat._Count);

            RunicHash = runicHash;
            Species = species;
            Color = color;
            PrimaryStats = primaryStats;
            PrimaryStatExp = new uint[(int) EPrimaryStat._Count];
            HiddenStats = hiddenStats;

            AgeInDays = 0;
            CurrentHealth = MaxHealth;
            CurrentStamina = MaxStamina;
            CurrentMorale = hiddenStats[(int) EHiddenStat.Morale];
        }
    }
}