using System;
using System.Windows;
using System.Windows.Controls;
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

        public void Setup(BitmapImage iconImage, string localizedText, Action callback, string tooltipContent = null)
        {
            _callback = callback;
            _icon.Source = iconImage;
            _label.Text = localizedText;
            _tooltipLabel.Content = tooltipContent;
            if (string.IsNullOrWhiteSpace(tooltipContent))
                _button.ToolTip = null;
        }

        public void Setup(string iconPath, string localizedText, Action callback, string tooltipContent = null)
        {
            Uri uri = new Uri(iconPath, UriKind.Relative);
            BitmapImage iconImage = new BitmapImage(uri);

            Setup(iconImage, localizedText, callback, tooltipContent);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            _callback?.Invoke();
        }
    }
}