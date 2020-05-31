using System;
using System.Windows;
using System.Windows.Controls;
using UAT_MS539.Code;

namespace UAT_MS539.Pages.Corral
{
    /// <summary>
    ///     Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
        {
            InitializeComponent();

            SetCryptid((uint) new Random().Next(100));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == buttonFeeding)
            {
                SetCryptid((uint) new Random().Next(100));
            }
            else if (sender == buttonToTown)
            {
            }
            else if (sender == buttonToColiseum)
            {
            }
        }

        private void SetCryptid(uint cryptidNumber)
        {
            imageCryptid.Source = AssetLoadUtility.LoadCryptid(cryptidNumber);
        }
    }
}