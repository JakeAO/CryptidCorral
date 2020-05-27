using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.Training
{
    public class TrainingRegimen
    {
        public string TrainingId;
        public string NameId;
        public Dictionary<EPrimaryStat, uint> GuaranteedStatIncrease;
        public Dictionary<EPrimaryStat, DropCalculation<uint>> RandomStatIncreases;

        public TrainingRegimen(
            Dictionary<EPrimaryStat, uint> guaranteedStatIncrease,
            Dictionary<EPrimaryStat, DropCalculation<uint>> randomStatIncreases)
        {
            GuaranteedStatIncrease = guaranteedStatIncrease;
            RandomStatIncreases = randomStatIncreases;
        }
    }
}