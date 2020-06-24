using System.Collections.Generic;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.Cryptid
{
    public class SpeciesDefinition
    {
        public string SpeciesId;
        public string NameId;
        public string ArtId;
        public uint SpawnRate;
        
        public DropCalculation<string> ColorFormula;
        
        public Dictionary<EPrimaryStat, DropCalculation<uint>> StartingStatFormulas;

        public SpeciesDefinition(string speciesId, string nameId, string artId,
            Dictionary<EPrimaryStat, DropCalculation<uint>> statFormulas,
            DropCalculation<string> colorFormula)
        {
            SpeciesId = speciesId;
            NameId = nameId;
            ArtId = artId;
            StartingStatFormulas = statFormulas;
            ColorFormula = colorFormula;
        }
    }
}