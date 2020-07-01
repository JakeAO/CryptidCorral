using System;
using System.Collections.Generic;
using System.Linq;
using Core.Code.Cryptid;
using Core.Code.StateMachine.CombatEngine;
using Core.Code.StateMachine.Interactions;
using Core.Code.StateMachine.Signals;
using Core.Code.Utility;

namespace Core.Code.StateMachine.States
{
    public class ColiseumResultsState : IState
    {
        public string LocationLocId => "Location/Coliseum";
        public string TimeLocId => "Time/Day";
        
        private Context _sharedContext;
        private PlayerData _playerData;
        private CombatData _combatData;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _playerData = context.Get<PlayerData>();
            _combatData = context.Get<CombatData>();
        }

        public void PerformContent(Context context)
        {
            if (_combatData.Victor == _playerData.ActiveCryptid)
            {
                HandleVictory();
            }
            else
            {
                HandleDefeat();
            }
        }

        public void PerformTeardown(Context context, IState nextState)
        {
            context.Clear<CombatData>();
        }

        private void CalculateCombatRewards(float globalMultiplier, out uint[] trainingPoints, out uint renown, out uint coins)
        {
            List<Cryptid.Cryptid> allOpponents = _combatData.Combatants.Where(x => x != _playerData.ActiveCryptid).ToList();

            uint playerCryptidWeight = WeightClassUtil.GetWeight(_playerData.ActiveCryptid);
            uint opponentCryptidWeight = (uint) allOpponents.Sum(x => WeightClassUtil.GetWeight(x));
            float weightMultiplier = opponentCryptidWeight / (float) playerCryptidWeight;

            uint baseCoinReward = 250;
            coins = (uint) Math.Ceiling(weightMultiplier * baseCoinReward * globalMultiplier);
            
            uint baseRenownReward = 10;
            renown = (uint) Math.Ceiling(weightMultiplier * baseRenownReward * globalMultiplier);

            trainingPoints = new uint[(int) EPrimaryStat._Count];
            uint baseExpReward = 200u + 200u * (uint) _combatData.BattleWeightClass;
            for (EPrimaryStat stat = EPrimaryStat.Strength; stat < EPrimaryStat._Count; stat++)
            {
                uint playerCryptidVal = _playerData.ActiveCryptid.PrimaryStats[(int) stat];
                uint opponentCryptidVal = (uint) allOpponents.Sum(x => x.PrimaryStats[(int) stat]);
                float bonusMultiplier = opponentCryptidVal / (float) playerCryptidVal;
                uint statExp = (uint) Math.Ceiling(bonusMultiplier * baseExpReward * globalMultiplier);

                trainingPoints[(int) stat] = statExp;
            }
        }

        private void HandleVictory()
        {
            CalculateCombatRewards(1.25f, out uint[] gainedTrainingPoints, out uint renownReward, out uint coinReward);

            DailyTrainingData trainingData = _sharedContext.Get<DailyTrainingData>();
            for (EPrimaryStat stat = EPrimaryStat.Strength; stat < EPrimaryStat._Count; stat++)
                trainingData.Points[(int) stat] += gainedTrainingPoints[(int) stat];
            _playerData.ActiveCryptid.CurrentRenown += renownReward;
            _playerData.ActiveCryptid.CurrentMorale = Math.Min(_playerData.ActiveCryptid.CurrentMorale + 10, _playerData.ActiveCryptid.HiddenStats[(int) EHiddenStat.Morale]);
            _playerData.Coins += coinReward;

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Coliseum/Victory",
                    new KeyValuePair<string, string>("{renown}", renownReward.ToString()),
                    new KeyValuePair<string, string>("{coins}", coinReward.ToString())),
                new UpdatePlayerData(_playerData),
                new DisplayTrainingResults(gainedTrainingPoints),
                new Option("Button/Next", OnNextSelected)
            });
        }

        private void HandleDefeat()
        {
            List<Cryptid.Cryptid> allOpponents = _combatData.Combatants.Where(x => x != _playerData.ActiveCryptid).ToList();
            float rewardMultiplier = 1f - allOpponents.Sum(x => x.CurrentHealth) / (float) allOpponents.Sum(x => x.MaxHealth);

            CalculateCombatRewards(rewardMultiplier, out uint[] gainedTrainingPoints, out uint renownReward, out uint coinReward);
            
            DailyTrainingData trainingData = _sharedContext.Get<DailyTrainingData>();
            for (EPrimaryStat stat = EPrimaryStat.Strength; stat < EPrimaryStat._Count; stat++)
                trainingData.Points[(int) stat] += gainedTrainingPoints[(int) stat];
            _playerData.ActiveCryptid.CurrentRenown += renownReward;
            _playerData.ActiveCryptid.CurrentMorale = Math.Max(_playerData.ActiveCryptid.CurrentMorale - 10, 0);
            _playerData.Coins += coinReward;

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Coliseum/Defeat",
                    new KeyValuePair<string, string>("{renown}", renownReward.ToString()),
                    new KeyValuePair<string, string>("{coins}", coinReward.ToString())),
                new UpdatePlayerData(_playerData),
                new DisplayTrainingResults(gainedTrainingPoints),
                new Option("Button/GoHome", OnGoHomeSelected)
            });
        }

        private void OnNextSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<ColiseumMainState>();
        }
        
        private void OnGoHomeSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<CorralNightState>();
        }
    }
}