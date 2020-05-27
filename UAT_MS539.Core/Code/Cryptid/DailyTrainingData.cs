using System;

namespace UAT_MS539.Core.Code.Cryptid
{
    public class DailyTrainingData
    {
        public Food.Food Food = null;
        public uint[] Points = new uint[(int) EPrimaryStat._Count];

        public uint[] CalculateStatIncreases()
        {
            uint[] statIncreases = new uint[(int) EPrimaryStat._Count];

            for (int i = 0; i < Points.Length; i++)
            {
                EPrimaryStat stat = (EPrimaryStat) i;

                uint boost = 0;
                float multiplier = 1f;

                if (Food != null)
                {
                    Food.BoostsByStat.TryGetValue(stat, out boost);
                    Food.MultipliersByStat.TryGetValue(stat, out multiplier);
                }

                statIncreases[i] = (uint) Math.Round(Points[i] * multiplier + boost);
            }

            return statIncreases;
        }
    }
}