using Core.Code.Utility;

namespace Core.Code.StateMachine.Interactions
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