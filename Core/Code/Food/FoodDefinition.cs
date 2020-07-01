using System.Collections.Generic;
using Core.Code.Cryptid;
using Core.Code.Utility;

namespace Core.Code.Food
{
    public class FoodDefinition
    {
        public string FoodId;
        public string NameId;
        public string ArtId;
        
        public uint SpawnRate = 1;
        
        public Dictionary<EPrimaryStat, DropCalculation<float>> StatGrowthMultipliers;
        public Dictionary<EPrimaryStat, DropCalculation<uint>> StatGrowthBoosts;
        public DropCalculation<uint> MoraleBoost;

        public FoodDefinition(string foodId, string nameId, string artId, uint spawnRate,
            Dictionary<EPrimaryStat, DropCalculation<uint>> boosts,
            Dictionary<EPrimaryStat, DropCalculation<float>> multipliers,
            DropCalculation<uint> moraleBoost)
        {
            FoodId = foodId;
            NameId = nameId;
            ArtId = artId;
            SpawnRate = spawnRate;
            StatGrowthBoosts = boosts;
            StatGrowthMultipliers = multipliers;
            MoraleBoost = moraleBoost;
        }
    }
}