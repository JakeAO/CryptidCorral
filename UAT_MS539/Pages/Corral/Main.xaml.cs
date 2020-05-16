using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UAT_MS539.Code;

namespace UAT_MS539.Pages.Corral
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
        {
            InitializeComponent();

            SetCryptid((uint)new Random().Next(100));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == buttonFeeding)
            {
                SetCryptid((uint)new Random().Next(100));
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
