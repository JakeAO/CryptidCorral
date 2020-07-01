using System.Collections.Generic;
using Core.Code.Utility;

namespace Core.Code.Cryptid
{
    public class SpeciesDefinition
    {
        public string SpeciesId;
        public string NameId;
        public string ArtId;
        public string MaskId;
        public uint SpawnRate;

        public DropCalculation<string> ColorFormula;

        public Dictionary<EPrimaryStat, DropCalculation<uint>> StartingStatFormulas;

        public SpeciesDefinition(
            string speciesId, string nameId,
            string artId, string maskId,
            uint spawnRate,
            Dictionary<EPrimaryStat, DropCalculation<uint>> statFormulas,
            DropCalculation<string> colorFormula)
        {
            SpeciesId = speciesId;
            NameId = nameId;
            ArtId = artId;
            MaskId = maskId;
            SpawnRate = spawnRate;
            StartingStatFormulas = statFormulas;
            ColorFormula = colorFormula;
        }
    }
}