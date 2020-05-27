namespace UAT_MS539.Core.Code.Cryptid
{
    public class CryptidDnaSample
    {
        public string SpeciesId;
        public string PatternId;
        public string ColorId;

        public uint[] PrimaryStats;
        public uint Renown;

        public CryptidDnaSample(string speciesId, string patternId, string colorId, uint[] primaryStats, uint renown)
        {
            SpeciesId = speciesId;
            PatternId = patternId;
            ColorId = colorId;
            PrimaryStats = primaryStats;
            Renown = renown;
        }
    }
}