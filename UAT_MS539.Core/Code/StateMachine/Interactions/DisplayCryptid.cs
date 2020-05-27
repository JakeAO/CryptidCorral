namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class DisplayCryptid : IInteraction
    {
        public readonly Cryptid.Cryptid Cryptid;

        public DisplayCryptid(Cryptid.Cryptid cryptid)
        {
            Cryptid = cryptid;
        }
    }
}