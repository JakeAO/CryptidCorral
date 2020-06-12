using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.Training
{
    public class TrainingRegimen
    {
        public string TrainingId;
        public string NameId;
        
        public uint StaminaCost;
        public uint SpawnRate = 1;
        
        public Dictionary<EPrimaryStat, uint> GuaranteedStatIncrease;
        public Dictionary<EPrimaryStat, DropCalculation<uint>> RandomStatIncreases;
        
        public TrainingRegimen(
            string trainingId, string nameId, uint staminaCost,
            Dictionary<EPrimaryStat, uint> guaranteedStatIncrease,
            Dictionary<EPrimaryStat, DropCalculation<uint>> randomStatIncreases)
        {
            TrainingId = trainingId;
            NameId = nameId;
            StaminaCost = staminaCost;
            GuaranteedStatIncrease = guaranteedStatIncrease;
            RandomStatIncreases = randomStatIncreases;
        }
    }
}