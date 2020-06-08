using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UAT_MS539.Core.Code.Cryptid;

namespace UAT_MS539.Components
{
    public partial class CryptidDisplay : UserControl
    {
        public CryptidDisplay()
        {
            InitializeComponent();
        }

        public void SetCryptid(Cryptid cryptid)
        {
            if (cryptid != null)
            {
                string baseDirectory = new Uri(AppDomain.CurrentDomain.BaseDirectory).PathAndQuery;
                string relativePath = cryptid.Species.ArtId.Remove(0, 1);
                string absolutePath = Path.Combine(baseDirectory, relativePath);
                Uri speciesArtPath = new Uri(absolutePath, UriKind.Absolute);
                BitmapImage cryptidImage = new BitmapImage(speciesArtPath);

                _baseFill.ImageSource = cryptidImage;
                _colorOpacity.ImageSource = cryptidImage;

                ColorDefinition colorDef = cryptid.Color;
                _colorFill.Color = Color.FromArgb(colorDef.A, colorDef.R, colorDef.G, colorDef.B);

                _cryptidScale.ScaleX = 1; // TODO: Flip horizontally once in a while?

                _mainCanvas.Visibility = Visibility.Visible;
            }
            else
            {
                _mainCanvas.Visibility = Visibility.Hidden;
            }
        }
    }
}