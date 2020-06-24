using System;
using System.Collections.Generic;
using System.Linq;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine.CombatEngine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class ColiseumMainState : IState
    {
        public string LocationLocId => "Location/Coliseum";
        public string TimeLocId => "Time/Day";

        private Random _random;
        
        private Context _sharedContext;
        private PlayerData _playerData;

        private static (uint, List<(Cryptid.Cryptid, EWeightClass)>) _selectionForDay = (0, null);

        public void PerformSetup(Context context, IState previousState)
        {
            _random = new Random();
            _sharedContext = context;
            _playerData = context.Get<PlayerData>();

            if (_playerData.ActiveCryptid != null &&
                (_selectionForDay == default ||
                 _selectionForDay.Item1 != _playerData.Day ||
                 _selectionForDay.Item2 == null))
            {
                _selectionForDay = (_playerData.Day, new List<(Cryptid.Cryptid, EWeightClass)>(9));
                
                SpeciesDatabase speciesDatabase = context.Get<SpeciesDatabase>();
                ColorDatabase colorDatabase = context.Get<ColorDatabase>();

                EWeightClass activeCryptidWeight = WeightClassUtil.GetWeightClass(_playerData.ActiveCryptid);
                EWeightClass minWeightClass = activeCryptidWeight > EWeightClass.E ? activeCryptidWeight - 1 : EWeightClass.E;
                EWeightClass maxWeightClass = activeCryptidWeight < EWeightClass.S ? activeCryptidWeight + 1 : EWeightClass.S;

                IEnumerable<string> hashGenBase = Enumerable.Repeat(NumberEncoder.Base24Encoding, 16);

                for (EWeightClass weightClass = minWeightClass; weightClass <= maxWeightClass; weightClass++)
                {
                    for (int newCryptidCount = 0; newCryptidCount < 3; newCryptidCount++)
                    {
                        string randomHash = new string(hashGenBase
                            .Select(x => x[_random.Next(x.Length)])
                            .ToArray());
                        uint randomWeight = WeightClassUtil.GetRandomWeight(weightClass, _random);

                        Cryptid.Cryptid newCryptid = CryptidUtilities.CreateCryptid(randomHash, speciesDatabase, colorDatabase);
                        uint startingWeight = WeightClassUtil.GetWeight(newCryptid);
                        uint remainingWeight = (uint) Math.Max((int) randomWeight - (int) startingWeight, 0);
                        for (uint statIncreaseCount = 0; statIncreaseCount < remainingWeight; statIncreaseCount++)
                        {
                            int randomStatIdx = _random.Next((int) EPrimaryStat._Count);
                            if (newCryptid.PrimaryStats[randomStatIdx] < CryptidUtilities.MAX_STAT_VAL)
                            {
                                newCryptid.PrimaryStats[randomStatIdx]++;
                            }
                            else
                            {
                                // Don't level a single stat over MAX_STAT_VAL (575)
                                statIncreaseCount--;
                            }
                        }

                        newCryptid.CurrentHealth = newCryptid.MaxHealth;
                        newCryptid.CurrentStamina = newCryptid.MaxStamina;
                        newCryptid.CurrentRenown = randomWeight / 11;
                        
                        _selectionForDay.Item2.Add((newCryptid, weightClass));
                    }
                }
            }
        }

        public void PerformContent(Context context)
        {
            if (_playerData.ActiveCryptid == null)
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Coliseum/NoCryptid"),
                    new Option("Button/GoHome", OnGoHomeSelected),
                });
            }
            else if (_playerData.ActiveCryptid.CurrentHealth == 0)
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Coliseum/NoHealthCryptid"),
                    new Option("Button/GoHome", OnGoHomeSelected),
                });
            }
            else
            {
                PromptForOpponent();
            }
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void PromptForOpponent()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Coliseum/Welcome"),
                new ColiseumOpponentSelection(_selectionForDay.Item2, OnOpponentSelected), 
                new Option("Button/GoHome", OnGoHomeSelected)
            });
        }

        private void OnOpponentSelected((Cryptid.Cryptid, EWeightClass) selection)
        {
            _selectionForDay.Item2.Remove(selection);
            
            CombatData combatData = new CombatData(_random, selection.Item2, _playerData.ActiveCryptid, selection.Item1);

            _sharedContext.Set(combatData);
            _sharedContext.Get<StateMachine>().ChangeState<ColiseumBattleState>();
        }

        private void OnGoHomeSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}