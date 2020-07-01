using System.Windows.Controls;

namespace AppWPF.Components
{
    public partial class CustomProgressBar : UserControl
    {
        public CustomProgressBar()
        {
            InitializeComponent();
        }

        public void SetProgress(float percent)
        {
            _fillGrid.Width = 90 * percent;
        }
    }
}
