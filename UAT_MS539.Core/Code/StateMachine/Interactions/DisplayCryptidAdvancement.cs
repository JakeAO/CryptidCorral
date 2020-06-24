namespace UAT_MS539.Core.Code.StateMachine.Interactions
{
    public class DisplayCryptidAdvancement : IInteraction
    {
        public readonly Cryptid.Cryptid Cryptid;
        public readonly uint[] PrimaryStatChanges;
        public readonly uint HealthChange;
        public readonly uint StaminaChange;

        public DisplayCryptidAdvancement(
            Cryptid.Cryptid cryptid,
            uint[] primaryStatChanges,
            uint healthChange,
            uint staminaChange)
        {
            Cryptid = cryptid;
            PrimaryStatChanges = primaryStatChanges;
            HealthChange = healthChange;
            StaminaChange = staminaChange;
        }
    }
}