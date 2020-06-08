using System;
using System.Windows.Controls;

namespace UAT_MS539.Components
{
    public partial class DefaultButton : UserControl
    {
        private Action _callback = null;

        public DefaultButton()
        {
            InitializeComponent();
        }

        public void Setup(string localizedText, Action callback)
        {
            _label.Text = localizedText;
            _callback = callback;
        }

        private void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _callback?.Invoke();
        }
    }
}
