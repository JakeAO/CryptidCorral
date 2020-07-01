using System;
using System.Windows.Controls;
using Core.Code.Utility;

namespace AppWPF.Components
{
    public partial class RunePatternDisplay : UserControl
    {
        private Action _callback = null;
        private readonly RuneDisplay[] _runeDisplays;

        public RunePatternDisplay()
        {
            InitializeComponent();

            _runeDisplays = new RuneDisplay[9];
            _runeDisplays[0] = _rune0;
            _runeDisplays[1] = _rune1;
            _runeDisplays[2] = _rune2;
            _runeDisplays[3] = _rune3;
            _runeDisplays[4] = _rune4;
            _runeDisplays[5] = _rune5;
            _runeDisplays[6] = _rune6;
            _runeDisplays[7] = _rune7;
            _runeDisplays[8] = _rune8;
        }

        public void SetPattern(RunePattern runePattern, RuneDatabase runeDatabase, Action callback)
        {
            _callback = callback;
            for (int i = 0; i < runePattern.RuneValues.Length; i++)
            {
                if (i >= _runeDisplays.Length)
                    break;
                _runeDisplays[i].SetRune(runePattern.RuneValues[i], runeDatabase);
            }
        }

        private void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _callback?.Invoke();
        }
    }
}