using System.Windows.Controls;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Components
{
    public partial class CryptidTooltip : UserControl
    {
        public CryptidTooltip()
        {
            InitializeComponent();
        }

        public void SetCryptid(Cryptid cryptid, LocDatabase locDatabase)
        {
            _speciesLabel.Content = locDatabase.Localize(cryptid.Species.NameId);

            _currentHealthLabel.Content = cryptid.CurrentHealth;
            _maxHealthLabel.Content = cryptid.MaxHealth;
            _currentStaminaLabel.Content = cryptid.CurrentStamina;
            _maxStaminaLabel.Content = cryptid.MaxStamina;
            _renownLabel.Content = cryptid.CurrentRenown;
            _ageLabel.Content = cryptid.AgeInDays;

            _strengthLabel.Content = cryptid.PrimaryStats[(int) EPrimaryStat.Strength];
            _strengthProgress.Value = cryptid.PrimaryStatExp[(int) EPrimaryStat.Strength];
            _speedLabel.Content = cryptid.PrimaryStats[(int) EPrimaryStat.Speed];
            _speedProgress.Value = cryptid.PrimaryStatExp[(int) EPrimaryStat.Speed];
            _vitalityLabel.Content = cryptid.PrimaryStats[(int) EPrimaryStat.Vitality];
            _vitalityProgress.Value = cryptid.PrimaryStatExp[(int) EPrimaryStat.Vitality];
            _skillLabel.Content = cryptid.PrimaryStats[(int) EPrimaryStat.Skill];
            _skillProgress.Value = cryptid.PrimaryStatExp[(int) EPrimaryStat.Skill];
            _smartsLabel.Content = cryptid.PrimaryStats[(int) EPrimaryStat.Smarts];
            _smartsProgress.Value = cryptid.PrimaryStatExp[(int) EPrimaryStat.Smarts];
            _luckLabel.Content = cryptid.PrimaryStats[(int) EPrimaryStat.Luck];
            _luckProgress.Value = cryptid.PrimaryStatExp[(int) EPrimaryStat.Luck];
        }
    }
}