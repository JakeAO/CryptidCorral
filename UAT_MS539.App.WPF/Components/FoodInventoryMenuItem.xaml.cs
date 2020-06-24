using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UAT_MS539.Code;
using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Components
{
    /// <summary>
    /// Interaction logic for FoodInventoryMenuItem.xaml
    /// </summary>
    public partial class FoodInventoryMenuItem : MenuItem
    {
        public FoodInventoryMenuItem(Food food, LocDatabase locDatabase) : base()
        {
            InitializeComponent();

            Header = locDatabase.Localize(food.Definition.NameId);
            _icon.Source = new BitmapImage(new Uri(food.Definition.ArtId, UriKind.Relative));
            _tooltip.Content = string.Join("\n", TooltipUtil.GetTooltipContent(food, locDatabase));
        }
    }
}
