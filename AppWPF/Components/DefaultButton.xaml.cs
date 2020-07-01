using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppWPF.Components
{
    public partial class DefaultButton : UserControl
    {
        public enum HighlightType
        {
            Default = 0,
            Good = 1,
            Bad = 2
        }

        private Action _callback = null;
        private ImageBrush[] _nineSliceBrushes = null;

        private DefaultButton(Action callback)
        {
            InitializeComponent();

            _callback = callback;
            _nineSliceBrushes = new ImageBrush[]
            {
                _rect0, _rect1, _rect2,
                _rect3, _rect4, _rect5,
                _rect6, _rect7, _rect8
            };
        }
        public DefaultButton(Action callback, string localizedText, string tooltipContent, HighlightType highlight = HighlightType.Default)
            : this(callback)
        {
            SetLabel(localizedText);
            SetIcon();
            SetTooltip(tooltipContent);
            SetHighlight(highlight);
        }
        public DefaultButton(Action callback, string iconPath, string localizedText, string tooltipContent, HighlightType highlight = HighlightType.Default)
            : this(callback, localizedText, tooltipContent, highlight)
        {
            SetIcon(iconPath);
        }
        public DefaultButton(Action callback, string localizedText, UserControl tooltipContent, HighlightType highlight = HighlightType.Default)
            : this(callback)
        {
            SetLabel(localizedText);
            SetIcon();
            SetTooltip(tooltipContent);
            SetHighlight(highlight);
        }
        public DefaultButton(Action callback, string iconPath, string localizedText, UserControl tooltipContent, HighlightType highlight = HighlightType.Default)
            : this(callback, localizedText, tooltipContent, highlight)
        {
            SetIcon(iconPath);
        }

        private void SetLabel(string localizedText)
        {
            _textLabel.Text = localizedText;
        }

        private void SetIcon() => SetIcon((BitmapImage)null);
        private void SetIcon(string bitmapPath)
        {
            if (bitmapPath.StartsWith("/") || bitmapPath.StartsWith("\\"))
                bitmapPath = bitmapPath.Remove(0, 1);
            SetIcon(new Uri(new Uri(AppDomain.CurrentDomain.BaseDirectory), bitmapPath));
        }
        private void SetIcon(Uri bitmapUri)
        {
            SetIcon(new BitmapImage(bitmapUri));
        }
        private void SetIcon(BitmapImage bitmapImage)
        {
            if (bitmapImage != null)
            {
                _iconImage.Source = bitmapImage;
                _iconImage.Visibility = Visibility.Visible;
            }
            else
            {
                _iconImage.Source = null;
                _iconImage.Visibility = Visibility.Collapsed;
            }
        }

        private void SetTooltip(string tooltipText)
        {
            _tooltipLabel.Content = tooltipText;
            if (string.IsNullOrWhiteSpace(tooltipText))
                _button.ToolTip = null;
        }
        private void SetTooltip(UserControl customTooltip)
        {
            _button.ToolTip = customTooltip;
        }

        private void SetHighlight(HighlightType highlight)
        {
            string targetPath = null;
            switch (highlight)
            {
                case HighlightType.Default:
                    targetPath = "Assets/UI/UI_Button_Grey.png";
                    break;
                case HighlightType.Good:
                    targetPath = "Assets/UI/UI_Button_Green.png";
                    break;
                case HighlightType.Bad:
                    targetPath = "Assets/UI/UI_Button_Red.png";
                    break;
            }

            Uri targetUri = new Uri(
                new Uri(AppDomain.CurrentDomain.BaseDirectory),
                targetPath);

            BitmapImage bitmap = new BitmapImage(targetUri);

            foreach (var brush in _nineSliceBrushes)
                brush.ImageSource = bitmap;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            _callback?.Invoke();
        }
    }
}
