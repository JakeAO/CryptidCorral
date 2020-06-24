using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UAT_MS539.Components
{
    public partial class ButtonWithIcon : UserControl
    {
        private Action _callback = null;

        public ButtonWithIcon()
        {
            InitializeComponent();
        }

        public void Setup(BitmapImage iconImage, string localizedText, Action callback, string tooltipContent = null, Color? highlight = null)
        {
            _callback = callback;
            _icon.Source = iconImage;
            _label.Text = localizedText;
            _tooltipLabel.Content = tooltipContent;
            if (string.IsNullOrWhiteSpace(tooltipContent))
                _button.ToolTip = null;
            if (highlight.HasValue)
                _button.Background = new SolidColorBrush(highlight.Value);
        }

        public void Setup(string iconPath, string localizedText, Action callback, string tooltipContent = null, Color? highlight = null)
        {
            Uri uri = new Uri(iconPath, UriKind.Relative);
            BitmapImage iconImage = new BitmapImage(uri);

            Setup(iconImage, localizedText, callback, tooltipContent, highlight);
        }

        public void Setup(BitmapImage iconImage, string localizedText, Action callback, UserControl customTooltip = null, Color? highlight = null)
        {
            _callback = callback;
            _icon.Source = iconImage;
            _label.Text = localizedText;
            _button.ToolTip = customTooltip;
            if (highlight.HasValue)
                _button.Background = new SolidColorBrush(highlight.Value);
        }

        public void Setup(string iconPath, string localizedText, Action callback, UserControl customTooltip = null, Color? highlight = null)
        {
            Uri uri = new Uri(iconPath, UriKind.Relative);
            BitmapImage iconImage = new BitmapImage(uri);

            Setup(iconImage, localizedText, callback, customTooltip, highlight);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            _callback?.Invoke();
        }
    }
}