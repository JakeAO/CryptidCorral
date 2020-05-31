using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;

namespace UAT_MS539.Core.Code.Food
{
    public class Food
    {
        public FoodDefinition Definition;
        
        public Dictionary<EPrimaryStat, float> MultipliersByStat;
        public Dictionary<EPrimaryStat, uint> BoostsByStat;
        public uint MoraleBoost;

        public float TotalFoodQuality;

        public Food(FoodDefinition definition,
            Dictionary<EPrimaryStat, uint> boostsByStat,
            Dictionary<EPrimaryStat, float> multipliersByStat,
            uint moraleBoost)
        {
            Definition = definition;
            BoostsByStat = boostsByStat;
            MultipliersByStat = multipliersByStat;
            MoraleBoost = moraleBoost;

            // TotalFoodQuality is used for internal purposes to compare Food instances against each other
            TotalFoodQuality = 0f;
            foreach (var boostValue in boostsByStat.Values) TotalFoodQuality += boostValue;
            foreach (var multiplierValue in multipliersByStat.Values) TotalFoodQuality *= multiplierValue;
            TotalFoodQuality += moraleBoost * 2.5f;
        }
    }
}