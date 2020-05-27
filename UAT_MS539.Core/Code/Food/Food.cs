using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;

namespace UAT_MS539.Core.Code.Food
{
    public class Food
    {
        public FoodDefinition Definition;
        public Dictionary<EPrimaryStat, uint> BoostsByStat;
        public Dictionary<EPrimaryStat, float> MultipliersByStat;

        public Food(FoodDefinition definition,
            Dictionary<EPrimaryStat, uint> boostsByStat,
            Dictionary<EPrimaryStat, float> multipliersByStat)
        {
            Definition = definition;
            BoostsByStat = boostsByStat;
            MultipliersByStat = multipliersByStat;
        }
    }
}