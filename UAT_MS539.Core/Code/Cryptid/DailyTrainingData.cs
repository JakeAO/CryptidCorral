using System;

namespace UAT_MS539.Core.Code.Cryptid
{
    public class DailyTrainingData
    {
        public Food.Food Food = null;
        public uint[] Points = new uint[(int) EPrimaryStat._Count];

        public void CalculateExpIncreases(out uint[] primaryStatIncreases, out uint moraleIncrease)
        {
            var expIncreases = new uint[(int) EPrimaryStat._Count];

            for (var i = 0; i < Points.Length; i++)
            {
                var stat = (EPrimaryStat) i;

                uint boost = 0;
                var multiplier = 1f;

                if (Food != null)
                {
                    Food.BoostsByStat.TryGetValue(stat, out boost);
                    Food.MultipliersByStat.TryGetValue(stat, out multiplier);
                }

                expIncreases[i] = (uint) Math.Round(Points[i] * multiplier + boost);
            }

            primaryStatIncreases = expIncreases;
            moraleIncrease = Food?.MoraleBoost ?? 0;
        }
    }
}