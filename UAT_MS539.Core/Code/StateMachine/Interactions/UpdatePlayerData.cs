using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class UpdatePlayerData : IInteraction
    {
        public readonly PlayerData PlayerData;

        public UpdatePlayerData(PlayerData playerData)
        {
            PlayerData = playerData;
        }
    }
}