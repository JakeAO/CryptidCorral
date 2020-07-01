using System;
using System.Collections.Generic;
using System.Linq;
using Core.Code.Cryptid;

namespace Core.Code.StateMachine.CombatEngine
{
    public class CombatEngine
    {
        public readonly struct CombatEvent
        {
            public readonly Cryptid.Cryptid Source;
            public readonly Cryptid.Cryptid Target;
            public readonly uint Damage;
            public readonly bool IsCrit;

            public CombatEvent(Cryptid.Cryptid source, Cryptid.Cryptid target, uint damage, bool crit)
            {
                Source = source;
                Target = target;
                Damage = damage;
                IsCrit = crit;
            }
        }

        private CombatData _combatData = null;

        public CombatEngine(CombatData data)
        {
            _combatData = data;
        }

        public IReadOnlyCollection<CombatEvent> ResolveCombat()
        {
            //// Combat ////
            // Strength -> Base Damage
            // Speed -> Initiative Order/Frequency (handled elsewhere)
            // Vitality -> Total HP (handled elsewhere)
            // Skill -> Maximum Bonus Damage (base 25% + 75% * Skill%)
            // Smarts -> Attack Success Rate (base 60% + 40% * Smarts%)
            // Luck -> Random Reroll Chance (Luck%, max of 5 times)
            
            Queue<CombatEvent> combatEvents = new Queue<CombatEvent>(100);
            do
            {
                // Get next combatant
                Cryptid.Cryptid source = _combatData.GetNextCombatant();

                // Get attack target
                Cryptid.Cryptid target = _combatData.Combatants.First(x => x != source && x.CurrentHealth > 0);
                
                // Perform attack
                float baseDamage = source.PrimaryStats[(int) EPrimaryStat.Strength];
                float maxBonusDamage = baseDamage * (0.25f + 0.75f * source.PrimaryStats[(int) EPrimaryStat.Skill] / CryptidUtilities.MAX_STAT_VAL);
                float attackSuccessRate = 0.6f + 0.4f * source.PrimaryStats[(int) EPrimaryStat.Smarts] / CryptidUtilities.MAX_STAT_VAL;
                float rerollChance = source.PrimaryStats[(int) EPrimaryStat.Luck] / CryptidUtilities.MAX_STAT_VAL;

                // Determine if attack is successful
                uint remainingSuccessAttackRerolls = 5;
                bool successfulAttack = false;
                do
                {
                    successfulAttack = _combatData.Random.NextDouble() < attackSuccessRate;
                    remainingSuccessAttackRerolls--;
                } while (!successfulAttack &&
                         remainingSuccessAttackRerolls > 0 &&
                         _combatData.Random.NextDouble() < rerollChance);
                
                // Determine damage amount of attack
                uint finalDamage = 0u;
                if (successfulAttack)
                {
                    uint damageBonusRerolls = 5;
                    float damageBonus = 0f;
                    do
                    {
                        float possibleDamageBonus = (float) (_combatData.Random.NextDouble() * maxBonusDamage);
                        if (possibleDamageBonus > damageBonus)
                            damageBonus = possibleDamageBonus;
                    } while (damageBonusRerolls > 0 &&
                             _combatData.Random.NextDouble() < rerollChance);

                    finalDamage = (uint) Math.Round(baseDamage + damageBonus);
                }

                // Apply attack effects
                target.CurrentHealth = (uint) Math.Max((int) target.CurrentHealth - (int) finalDamage, 0);
                
                // Add attack to event queue
                bool isCrit = finalDamage > (baseDamage + maxBonusDamage) * 0.9f;
                combatEvents.Enqueue(new CombatEvent(source, target, finalDamage, isCrit));
                
                // End combat if one combatant remains
            } while (_combatData.Combatants.Count(x => x.CurrentHealth > 0) > 1);

            // Victor is the last cryptid standing
            _combatData.Victor = _combatData.Combatants.First(x => x.CurrentHealth > 0);

            return combatEvents;
        }
    }
}