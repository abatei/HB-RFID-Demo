using HBLib;
using HBLib.HR8002Reader;
using HBLib.ISO15693;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace I15693FormDemo
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

        ComPort com = new ComPort("COM3", 19200, 300);
        Reader reader;
        I15693 i15693;
        bool isFirst = true;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            com.Open();
            reader = new Reader(0x00, com);
            i15693 = new I15693(0x00, com);
            reader.ChangeToISO15693Async();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InventoryScan();
        }

        private async Task InventoryScan()
        {
            InventoryScanInfo info;
            if (isFirst)
            {   //新的扫描
                info = await i15693.InventoryScanWithoutAFIAsync(InventoryScanWithoutAFIMode.NewScanWithoutAFI);
                isFirst = false;
            }
            else
            {   //继续扫描
                info = await i15693.InventoryScanWithoutAFIAsync(InventoryScanWithoutAFIMode.ContinueScanWithoutAFI);
            }
            this.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var card in info.CardSet)
                {
                    lstCard.Items.Add(card.GetUIDStr());
                }
            }));
        }
    }
}
