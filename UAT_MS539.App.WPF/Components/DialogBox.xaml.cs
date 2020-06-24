using System.Windows.Controls;

namespace UAT_MS539.Components
{
    public partial class DialogBox : UserControl
    {
        public DialogBox()
        {
            InitializeComponent();
        }

        public void SetLabel(string localizedText)
        {
            _label.Text = localizedText;
            _scrollViewer.ScrollToTop();
        }
    }
}
