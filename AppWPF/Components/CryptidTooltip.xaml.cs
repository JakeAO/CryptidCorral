using System.Windows.Controls;
using Core.Code.Cryptid;
using Core.Code.Utility;

namespace AppWPF.Components
{
    public partial class CryptidTooltip : UserControl
    {
        public CryptidTooltip()
        {
            InitializeComponent();
        }
        public CryptidTooltip(Cryptid cryptid, LocDatabase locDatabase) : this()
        {
            SetCryptid(cryptid, locDatabase);
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

            _strengthLabel.Content = cryptid.PrimaryStats[(int)EPrimaryStat.Strength];
            _strengthProgress.SetProgress(cryptid.PrimaryStatExp[(int)EPrimaryStat.Strength] / 100f);
            _speedLabel.Content = cryptid.PrimaryStats[(int)EPrimaryStat.Speed];
            _speedProgress.SetProgress(cryptid.PrimaryStatExp[(int)EPrimaryStat.Speed] / 100f);
            _vitalityLabel.Content = cryptid.PrimaryStats[(int)EPrimaryStat.Vitality];
            _vitalityProgress.SetProgress(cryptid.PrimaryStatExp[(int)EPrimaryStat.Vitality] / 100f);
            _skillLabel.Content = cryptid.PrimaryStats[(int)EPrimaryStat.Skill];
            _skillProgress.SetProgress(cryptid.PrimaryStatExp[(int)EPrimaryStat.Skill] / 100f);
            _smartsLabel.Content = cryptid.PrimaryStats[(int)EPrimaryStat.Smarts];
            _smartsProgress.SetProgress(cryptid.PrimaryStatExp[(int)EPrimaryStat.Smarts] / 100f);
            _luckLabel.Content = cryptid.PrimaryStats[(int)EPrimaryStat.Luck];
            _luckProgress.SetProgress(cryptid.PrimaryStatExp[(int)EPrimaryStat.Luck] / 100f);
        }
    }
}