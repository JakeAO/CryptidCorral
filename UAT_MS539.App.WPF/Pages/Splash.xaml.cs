using System.Windows;
using System.Windows.Controls;
using UAT_MS539.Pages.Corral;

namespace UAT_MS539.Pages
{
    /// <summary>
    ///     Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : UserControl
    {
        public Splash()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == buttonToCorral) (Window.GetWindow(this) as MainWindow).ChangeContent(new Main());
        }
    }
}