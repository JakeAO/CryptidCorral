using System;
using System.Collections.Generic;
using System.Linq;
using Core.Code.Cryptid;

namespace Core.Code.StateMachine.CombatEngine
{
    public class CombatData
    {
        private class InitiativeData
        {
            public Cryptid.Cryptid Cryptid;
            public int Initiative;
            public int MinStep;
            public int MaxStep;
        }

        public readonly Random Random = null;
        public readonly EWeightClass BattleWeightClass = EWeightClass.E;
        public readonly IReadOnlyList<Cryptid.Cryptid> Combatants = null;
        public readonly IReadOnlyDictionary<Cryptid.Cryptid, uint> StartingHealthTotals = null;
        public Cryptid.Cryptid Victor = null;

        private readonly List<InitiativeData> _turnOrder = null;

        public CombatData(Random random, EWeightClass battleWeightClass, params Cryptid.Cryptid[] combatants)
        {
            Dictionary<Cryptid.Cryptid, uint> startingHealthTotals = new Dictionary<Cryptid.Cryptid, uint>();

            Random = random;
            BattleWeightClass = battleWeightClass;
            Combatants = combatants;
            StartingHealthTotals = startingHealthTotals;
            
            _turnOrder = new List<InitiativeData>();
            foreach (Cryptid.Cryptid combatant in combatants)
            {
                startingHealthTotals[combatant] = combatant.CurrentHealth;
                
                int maxMinusStat = (int) (CryptidUtilities.MAX_STAT_VAL - combatant.PrimaryStats[(int) EPrimaryStat.Speed]);
                _turnOrder.Add(new InitiativeData()
                {
                    Cryptid = combatant,
                    Initiative = maxMinusStat,
                    MinStep = maxMinusStat,
                    MaxStep = 100 + maxMinusStat
                });
            }
        }

        public Cryptid.Cryptid GetNextCombatant()
        {
            if (!_turnOrder.Any(x => x.Initiative <= 0))
            {
                int minInit = _turnOrder.Min(x => x.Initiative);
                foreach (var initiativeData in _turnOrder)
                {
                    initiativeData.Initiative -= minInit;
                }
            }

            InitiativeData nextInit = _turnOrder.First(x => x.Initiative <= 0);
            nextInit.Initiative += Random.Next(nextInit.MinStep, nextInit.MaxStep);
            return nextInit.Cryptid;
        }
    }
}