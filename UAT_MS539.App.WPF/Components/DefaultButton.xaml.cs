using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UAT_MS539.Components
{
    public partial class DefaultButton : UserControl
    {
        private Action _callback = null;

        public DefaultButton()
        {
            InitializeComponent();
        }

        public void Setup(string localizedText, Action callback, string tooltipContent = null, Color? highlight = null)
        {
            _label.Text = localizedText;
            _callback = callback;
            _tooltipLabel.Content = tooltipContent;
            if (string.IsNullOrWhiteSpace(tooltipContent))
                _button.ToolTip = null;
            if (highlight.HasValue)
                _button.Background = new SolidColorBrush(highlight.Value);
        }

        private void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _callback?.Invoke();
        }
    }
}
