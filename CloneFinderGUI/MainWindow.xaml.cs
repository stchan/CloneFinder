using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CloneFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Ctor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Object level variables

        #endregion

        #region Control Event Handlers
        private void menuHelpAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuFileExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        #endregion

        #region Window Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void InitializeTreeView()
        {
            DriveInfo[] systemDrives = DriveInfo.GetDrives();
            for (int loop = 0; loop <= systemDrives.GetUpperBound(0); loop++)
            {
                
            }
        }

        #endregion
    }
}
