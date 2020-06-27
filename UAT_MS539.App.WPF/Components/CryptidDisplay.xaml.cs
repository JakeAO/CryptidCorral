using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UAT_MS539.Code;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Components
{
    public partial class CryptidDisplay : UserControl
    {
        public CryptidDisplay()
        {
            InitializeComponent();
        }

        public void SetVisibility(bool visible)
        {
            _mainCanvas.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
        
        public void SetCryptid(Cryptid cryptid, LocDatabase locDatabase, Logger logger, bool faceRight = true)
        {
            if (cryptid != null)
            {
                Uri baseDirectory = new Uri(AppDomain.CurrentDomain.BaseDirectory);
                string relativePath = cryptid.Species.ArtId.Remove(0, 1);
                Uri speciesArtPath = new Uri(baseDirectory, relativePath);
                
                try
                {
                    BitmapImage cryptidImage = new BitmapImage(speciesArtPath);
                    _baseFill.ImageSource = cryptidImage;
                    _colorOpacity.ImageSource = cryptidImage;
                }
                catch (Exception e)
                {
                    logger?.Log(Logger.LogLevel.Exception, $"{e.GetType().Name}: {e.Message} ({speciesArtPath})");
                }

                ColorDefinition colorDef = cryptid.Color;
                _colorFill.Color = Color.FromArgb(colorDef.A, colorDef.R, colorDef.G, colorDef.B);

                _tooltipPanel.SetCryptid(cryptid, locDatabase);

                _cryptidScale.ScaleX = faceRight ? 1 : -1;
            }

            SetVisibility(cryptid != null);
        }
    }
}