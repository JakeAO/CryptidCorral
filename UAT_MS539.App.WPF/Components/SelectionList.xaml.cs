using System.Windows.Controls;

namespace UAT_MS539.Components
{
    public partial class SelectionList : UserControl
    {
        public SelectionList()
        {
            InitializeComponent();
        }

        public void AddSelectable(UserControl userControl)
        {
            _stackPanel.Children.Add(userControl);
        }

        public void ClearAll()
        {
            _stackPanel.Children.Clear();
        }
    }
}
