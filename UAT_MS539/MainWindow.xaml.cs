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
using UAT_MS539.Code.DataTypes;
using UAT_MS539.Pages;

namespace UAT_MS539
{
    /// <summary>
    /// Interaction logic for Window.xaml
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