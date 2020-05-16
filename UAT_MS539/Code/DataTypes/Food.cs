using System;
using System.Collections.Generic;
using System.Text;

namespace UAT_MS539.Code.DataTypes
{
    public enum EFoodCategory
    {
        Meat,
        Fish,
        Bread,
        Fruit,
        Vegetable,
        Dairy,
        Treat
    }

    public class Food
    {
        public readonly EFoodCategory Category;
        public readonly string Name;
        public readonly Dictionary<EPrimaryStat, float> StatGrowthBoosts;
        public readonly Dictionary<EPrimaryStat, float> StatGrowthMultipliers;
    }
}
