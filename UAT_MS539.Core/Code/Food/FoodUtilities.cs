using System;
using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;

namespace UAT_MS539.Core.Code.Food
{
    public static class FoodUtilities
    {
        private static readonly Random _random = new Random();

        public static Food CreateFood(FoodDefinition definition)
        {
            Dictionary<EPrimaryStat, uint> boosts = new Dictionary<EPrimaryStat, uint>(definition.StatGrowthBoosts.Count);
            foreach (var statBoostKvp in definition.StatGrowthBoosts)
            {
                boosts[statBoostKvp.Key] = statBoostKvp.Value.Evaluate((float) _random.NextDouble());
            }

            Dictionary<EPrimaryStat, float> multipliers = new Dictionary<EPrimaryStat, float>(definition.StatGrowthMultipliers.Count);
            foreach (var statMultiplierKvp in definition.StatGrowthMultipliers)
            {
                multipliers[statMultiplierKvp.Key] = statMultiplierKvp.Value.Evaluate((float) _random.NextDouble());
            }

            return new Food(definition, boosts, multipliers);
        }
    }
}