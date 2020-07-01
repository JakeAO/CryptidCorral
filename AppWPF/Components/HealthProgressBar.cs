using System;
using System.Windows.Controls;

namespace AppWPF.Components
{
    public partial class HealthProgressBar : UserControl
    {
        public HealthProgressBar()
        {
            InitializeComponent();
        }

        public void SetProgress(float current, float max)
        {
            current = Math.Max(0f, current);
            _fillGrid.Width = 240 * current / max;
            _textLabel.Text = $"{current}/{max}";
        }
    }
}
