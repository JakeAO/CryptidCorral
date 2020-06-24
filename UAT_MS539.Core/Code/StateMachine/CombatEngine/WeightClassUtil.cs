using System;
using System.Collections.Generic;
using System.Linq;

namespace UAT_MS539.Core.Code.StateMachine.CombatEngine
{
    public static class WeightClassUtil
    {
        private static readonly IReadOnlyDictionary<EWeightClass, uint> _minWeight = new Dictionary<EWeightClass, uint>()
        {
            {EWeightClass.S, 1800u},
            {EWeightClass.A, 1200u},
            {EWeightClass.B, 600u},
            {EWeightClass.C, 300u},
            {EWeightClass.D, 100u},
            {EWeightClass.E, 0u}
        };

        private static readonly IReadOnlyDictionary<EWeightClass, uint> _maxWeight = new Dictionary<EWeightClass, uint>()
        {
            {EWeightClass.S, (uint) (Cryptid.CryptidUtilities.MAX_STAT_VAL * 6u)},
            {EWeightClass.A, 1799u},
            {EWeightClass.B, 1199u},
            {EWeightClass.C, 599u},
            {EWeightClass.D, 299u},
            {EWeightClass.E, 99u}
        };

        public static uint GetWeight(Cryptid.Cryptid cryptid)
        {
            return (uint) cryptid.PrimaryStats.Sum(x => x);
        }

        public static EWeightClass GetWeightClass(Cryptid.Cryptid cryptid)
        {
            uint totalStats = GetWeight(cryptid);
            if (totalStats >= _minWeight[EWeightClass.S])
                return EWeightClass.S;
            if (totalStats >= _minWeight[EWeightClass.A])
                return EWeightClass.A;
            if (totalStats >= _minWeight[EWeightClass.B])
                return EWeightClass.B;
            if (totalStats >= _minWeight[EWeightClass.C])
                return EWeightClass.C;
            if (totalStats >= _minWeight[EWeightClass.D])
                return EWeightClass.D;

            return EWeightClass.E;
        }

        public static uint GetRandomWeight(EWeightClass weightClass, Random random)
        {
            return (uint) random.Next(
                (int) _minWeight[weightClass],
                (int) _maxWeight[weightClass]);
        }
    }
}