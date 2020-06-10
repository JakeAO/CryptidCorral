namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class DisplayCoins : IInteraction
    {
        public readonly uint Coins;

        public DisplayCoins(uint coins)
        {
            Coins = coins;
        }
    }
}