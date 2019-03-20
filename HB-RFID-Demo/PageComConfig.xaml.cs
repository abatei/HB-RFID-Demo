using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO.Ports;
using HBLib;

namespace HB_RFID_Demo
{
    /// <summary>
    /// PageComConfig.xaml 的交互逻辑
    /// </summary>
    public partial class PageComConfig : Page
    {
        public PageComConfig()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (var p in ports)
            {
                cbPortName.Items.Add(p);
            }
            cbPortName.SelectedIndex = 0;
        }

        SolidColorBrush blackBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
        SolidColorBrush greenBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xA0, 0xBB, 0x55));

        private void BtnCom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (btnCom.Text == "启动")
            {
                if (cbPortName.Text == "")
                {
                    MessageBox.Show("请先选择一个串口号！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (MainWindow.com == null)
                {
                    MainWindow.com = new ComPort(cbPortName.Text, int.Parse(lblBaudRate.Text), 300);
                }

                if (MainWindow.com.Open() != ReturnMessage.Success)
                {
                    MessageBox.Show("打开串口失败","错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                btnCom.Background = greenBrush;
                btnCom.Text = "关闭";
            }
            else
            {
                MainWindow.com.Close();
                btnCom.Background = blackBrush;
                btnCom.Text = "启动";
            }
        }        
    }
}
