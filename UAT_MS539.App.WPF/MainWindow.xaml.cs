using System.Windows;
using System.Windows.Controls;
using UAT_MS539.Pages;

namespace UAT_MS539
{
    /// <summary>
    ///     Interaction logic for Window.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ChangeContent(new Splash());
        }

        public void ChangeContent(UserControl newContent)
        {
            contentControl.Content = newContent;
        }
    }
}