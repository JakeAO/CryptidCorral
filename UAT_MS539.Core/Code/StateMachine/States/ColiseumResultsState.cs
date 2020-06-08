using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class ColiseumResultsState : IState
    {
        public string LocationLocId => "Location/Coliseum";
        public string TimeLocId => "Time/Day";

        private Context _sharedContext;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
        }

        public void PerformContent(Context context)
        {
            Cryptid.Cryptid playerCryptid = null; //context.Get<ColiseumBattleData>().PlayerCryptid
            Cryptid.Cryptid opponentCryptid = null; //context.Get<ColiseumBattleData>().OpponentCryptid
            var playerDidWin = true; //context.Get<ColiseumBattleData>().PlayerDidWin
            var expGained = new uint[(int) EPrimaryStat._Count]; //context.Get<ColiseumBattleData>().ExpGained

            context.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("[TEMP] Your Cryptid did good/bad, here are the results."),
                //new DisplayColiseumResults(playerCryptid, opponentCryptid, playerDidWin, expGained),
                new Option("Button/Back", OnBackSelected)
            });
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void OnBackSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<ColiseumMainState>();
        }
    }
}