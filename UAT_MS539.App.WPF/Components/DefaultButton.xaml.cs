using System;
using System.Windows;
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

        public void Setup(string localizedText, Action callback, string tooltipContent = null)
        {
            _label.Text = localizedText;
            _callback = callback;
            _tooltipLabel.Content = tooltipContent;
            if (string.IsNullOrWhiteSpace(tooltipContent))
                _button.ToolTip = null;
        }

        private void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _callback?.Invoke();
        }
    }
}
