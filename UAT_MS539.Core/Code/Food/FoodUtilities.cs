using System;
using System.Collections.Generic;
using System.Linq;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.Food
{
    public static class FoodUtilities
    {
        private static readonly Random _random = new Random();

        private static readonly FoodDefinition _basicRationDefinition = new FoodDefinition(
            "BasicRation", "Food/BasicRation", "/Assets/Food/food_basic.png", 0,
            new Dictionary<EPrimaryStat, DropCalculation<uint>>(),
            new Dictionary<EPrimaryStat, DropCalculation<float>>
            {
                {
                    EPrimaryStat.Strength, new DropCalculation<float>(
                        new DropCalculation<float>.Point(1f, 6),
                        new DropCalculation<float>.Point(1.1f, 3),
                        new DropCalculation<float>.Point(1.2f, 1))
                },
                {
                    EPrimaryStat.Vitality, new DropCalculation<float>(
                        new DropCalculation<float>.Point(1f, 6),
                        new DropCalculation<float>.Point(1.1f, 3),
                        new DropCalculation<float>.Point(1.2f, 1))
                },
                {
                    EPrimaryStat.Speed, new DropCalculation<float>(
                        new DropCalculation<float>.Point(1f, 6),
                        new DropCalculation<float>.Point(1.1f, 3),
                        new DropCalculation<float>.Point(1.2f, 1))
                },
                {
                    EPrimaryStat.Smarts, new DropCalculation<float>(
                        new DropCalculation<float>.Point(1f, 6),
                        new DropCalculation<float>.Point(1.1f, 3),
                        new DropCalculation<float>.Point(1.2f, 1))
                },
                {
                    EPrimaryStat.Skill, new DropCalculation<float>(
                        new DropCalculation<float>.Point(1f, 6),
                        new DropCalculation<float>.Point(1.1f, 3),
                        new DropCalculation<float>.Point(1.2f, 1))
                },
                {
                    EPrimaryStat.Luck, new DropCalculation<float>(
                        new DropCalculation<float>.Point(1f, 6),
                        new DropCalculation<float>.Point(1.1f, 3),
                        new DropCalculation<float>.Point(1.2f, 1))
                }
            },
            new DropCalculation<uint>());

        public static Food CreateBasicRation()
        {
            Food newFood;
            do
            {
                newFood = CreateFood(_basicRationDefinition);
            } while (newFood.MultipliersByStat.Values.All(x => x == 1f));

            newFood.TotalFoodQuality = 0f;
            
            return newFood;
        }

        public static Food CreateFood(FoodDefinition definition)
        {
            var boosts = new Dictionary<EPrimaryStat, uint>(definition.StatGrowthBoosts.Count);
            foreach (var statBoostKvp in definition.StatGrowthBoosts) boosts[statBoostKvp.Key] = statBoostKvp.Value.Evaluate((float) _random.NextDouble());

            var multipliers = new Dictionary<EPrimaryStat, float>(definition.StatGrowthMultipliers.Count);
            foreach (var statMultiplierKvp in definition.StatGrowthMultipliers) multipliers[statMultiplierKvp.Key] = statMultiplierKvp.Value.Evaluate((float) _random.NextDouble());

            var moraleBoost = definition.MoraleBoost?.Evaluate((float) _random.NextDouble()) ?? 0;

            return new Food(definition, boosts, multipliers, moraleBoost);
        }
    }
}