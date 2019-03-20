using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HBLib;
using HBLib.HR8002Reader;

namespace HB_RFID_Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public static ComPort com;
        PageComConfig pageComConfig;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pageComConfig = new PageComConfig();
            ccPage.Content = new Frame() { Content = pageComConfig };
        }
        private void MI_ComConfig_Click(object sender, RoutedEventArgs e)
        {
            if (pageComConfig == null)
            {
                pageComConfig = new PageComConfig();
            }
            ccPage.Content = new Frame() { Content = pageComConfig };
        }

        PageSelMulti pageSelMulti;
        private void MI_SelMulti_Click(object sender, RoutedEventArgs e)
        {
            if (pageSelMulti == null)
            {
                pageSelMulti = new PageSelMulti();
            }
            ccPage.Content = new Frame() { Content = pageSelMulti };
        }

        PageAuthentication pageAuthentication;
        private void MI_Authentication_Click(object sender, RoutedEventArgs e)
        {
            if (pageAuthentication == null)
            {
                pageAuthentication = new PageAuthentication();
            }
            ccPage.Content = new Frame() { Content = pageAuthentication };
        }
    }
}
