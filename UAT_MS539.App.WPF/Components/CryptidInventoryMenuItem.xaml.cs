using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Components
{
    /// <summary>
    /// Interaction logic for CryptidInventoryMenuItem.xaml
    /// </summary>
    public partial class CryptidInventoryMenuItem : MenuItem
    {
        public CryptidInventoryMenuItem(Cryptid cryptid, LocDatabase locDatabase, CryptidTooltip cryptidTooltip) : base()
        {
            InitializeComponent();

            Header = locDatabase.Localize(cryptid.Species.NameId);
            _icon.Source = new BitmapImage(new Uri(cryptid.Species.ArtId, UriKind.Relative));
            ToolTip = cryptidTooltip;
        }
    }
}
