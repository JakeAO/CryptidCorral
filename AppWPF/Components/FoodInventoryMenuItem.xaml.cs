using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AppWPF.Code;
using Core.Code.Food;
using Core.Code.Utility;

namespace AppWPF.Components
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
