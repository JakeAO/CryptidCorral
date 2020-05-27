using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.Food
{
    public class FoodDefinition
    {
        public string FoodId;
        public string NameId;
        public string ArtId;
        public Dictionary<EPrimaryStat, DropCalculation<uint>> StatGrowthBoosts;
        public Dictionary<EPrimaryStat, DropCalculation<float>> StatGrowthMultipliers;

        public FoodDefinition(string foodId, string nameId, string artId,
            Dictionary<EPrimaryStat, DropCalculation<uint>> boosts,
            Dictionary<EPrimaryStat, DropCalculation<float>> multipliers)
        {
            FoodId = foodId;
            NameId = nameId;
            ArtId = artId;
            StatGrowthBoosts = boosts;
            StatGrowthMultipliers = multipliers;
        }
    }
}