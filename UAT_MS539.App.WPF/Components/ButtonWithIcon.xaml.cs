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

        public void Setup(BitmapImage iconImage, string localizedText, Action callback)
        {
            _callback = callback;
            _icon.Source = iconImage;
            _label.Text = localizedText;
        }

        public void Setup(string iconPath, string localizedText, Action callback)
        {
            Uri uri = new Uri(iconPath, UriKind.Relative);
            BitmapImage iconImage = new BitmapImage(uri);

            Setup(iconImage, localizedText, callback);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            _callback?.Invoke();
        }
    }
}
