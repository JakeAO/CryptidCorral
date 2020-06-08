using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Components
{
    public partial class RuneDisplay : UserControl
    {
        public RuneDisplay()
        {
            InitializeComponent();
        }

        public void SetRune(uint runeValue, RuneDatabase runeDatabase)
        {
            if (runeDatabase.RuneById.TryGetValue(runeValue, out var runeDefinition))
            {
                Uri runeArtPath = new Uri(runeDefinition.ArtId, UriKind.Relative);
                BitmapImage runeImage = new BitmapImage(runeArtPath);
                _runeGlyph.Source = runeImage;
            }
            else
            {
                _runeGlyph.Source = null;
            }
        }
    }
}