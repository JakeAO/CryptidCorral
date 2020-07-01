using System.Windows.Controls;

namespace AppWPF.Components
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
