namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class DisplayCryptidAdvancement : IInteraction
    {
        public readonly Cryptid.Cryptid Cryptid;
        public readonly uint[] PrimaryStatChanges;
        public readonly uint[] SecondaryStatChanges;

        public DisplayCryptidAdvancement(
            Cryptid.Cryptid cryptid,
            uint[] primaryStatChanges,
            uint[] secondaryStatChanges)
        {
            Cryptid = cryptid;
            PrimaryStatChanges = primaryStatChanges;
            SecondaryStatChanges = secondaryStatChanges;
        }
    }
}