using HBLib;
using HBLib.HR8002Reader;
using HBLib.ISO14443A;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ValueOper
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
        const byte OPER_BLOCKNUM = 1;
        ComPort com = new ComPort("COM6", 19200, 300);
        Reader reader;
        I14443A i14443a;
        byte[] operUID; //上一轮操作被扣钱的标签ID
        CancellationTokenSource ts;
        SolidColorBrush grayBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x55, 0x55, 0x55));
        SolidColorBrush greenBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xA0, 0xBB, 0x55));

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            com.Open();
            reader = new Reader(0x00, com);
            i14443a = new I14443A(0x00, com);
            reader.ChangeToISO14443AAsync();
        }

        private void Btn_InitValue_Click(object sender, RoutedEventArgs e)
        {
            InitValue(50, OPER_BLOCKNUM); //充值 50 块钱
        }
        //开线程等待刷卡扣钱
        private void Btn_Work_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(Btn_Work.Content) == "进入工作状态") //开线程
            {
                ts = new CancellationTokenSource();
                Task.Factory.StartNew(() => Work(6, OPER_BLOCKNUM), ts.Token,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
                Btn_Work.Background = greenBrush;
                Btn_Work.Content = "退出工作状态";
                BtnInit.IsEnabled = false;
                BtnAdd.IsEnabled = false;
            }
            else //取消线程
            {
                ts.Cancel();
                Btn_Work.Background = grayBrush;
                Btn_Work.Content = "进入工作状态";
                BtnInit.IsEnabled = true;
                BtnAdd.IsEnabled = true;
            }
        }

        private void Btn_Increment_Click(object sender, RoutedEventArgs e)
        {
            Increment(int.Parse(txtAddCoin.Text), OPER_BLOCKNUM);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="coin">充入的钱数，块为单位</param>
        /// <param name="blockNum">操作的绝对块号</param>
        /// <returns></returns>
        private async Task Increment(int coin, byte blockNum)
        {
            // 选卡操作
            byte[] uid = await Select();
            if (uid == null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.Text = "没有选中任何卡片！";
                }));
                return;
            }
            //证实
            if (!await AuthKey((byte)(blockNum / 4)))
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.Text = "证实失败！";
                }));
                return;
            }
            byte[] data = BitConverter.GetBytes(coin * 100);
            var info = await i14443a.IncrementAsync(blockNum, data);
            if (info.ReturnValue == ReturnMessage.Success)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.Text = "充值成功，已充入 " + coin.ToString() + "元钱。";
                }));
                //读取剩余钱数
                var info1 = await i14443a.ReadValueAsync(blockNum);
                if (info1.ReturnValue == ReturnMessage.Success)
                {
                    int remain = BitConverter.ToInt32(info1.ValueData, 0);
                    string money = Convert.ToString(remain / 100);
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        txtMsg.AppendText("卡内剩余：" + money + " 块钱。");
                    }));
                }
            }
        }

        /// <summary>
        /// 让程序处于工作状态，等待刷卡并扣钱
        /// </summary>
        /// <param name="coin">每次刷卡扣的钱数</param>
        /// <returns></returns>
        private async Task Work(int coin, byte blockNum)
        {
            byte[] data = BitConverter.GetBytes(coin * 100);
            while (!ts.IsCancellationRequested)
            {
                // 选卡
                byte[] uid = await Select();
                if (uid != null)
                {
                    if (CompareUID(uid, operUID)) //如果此卡之前已经扣过钱且未离开感应区，则不操作
                    {
                        continue;
                    }
                }
                else
                {
                    operUID = null; //说明卡片已经离开感应区
                    continue;
                }
                //证实
                if (!await AuthKey((byte)(blockNum / 4))) continue;
                //读取剩余钱数，判断卡内剩余的钱是否够扣
                var info1 = await i14443a.ReadValueAsync(blockNum);
                if (info1.ReturnValue == ReturnMessage.Success)
                {
                    int remain = BitConverter.ToInt32(info1.ValueData, 0);
                    if (remain < coin * 100)
                    {
                        string money = Convert.ToString(remain / 100);
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            txtMsg.AppendText("\r\n卡内余额不足，现剩余：" + money + " 块钱。");
                        }));
                        operUID = uid;
                        continue;
                    }

                }

                var info = await i14443a.ValueAsync(OperCode.Decrease, blockNum, data, blockNum); //扣钱
                if (info.ReturnValue != ReturnMessage.Success) continue;
                operUID = uid; //记录被扣钱的标签UID，以防下次重复扣钱
                await reader.BeepAsync(5, 0, 1); //蜂鸣器响一声
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.AppendText("\r\n成功刷卡，扣除" + coin.ToString() + "块钱。");
                }));
                //读取剩余钱数
                info1 = await i14443a.ReadValueAsync(blockNum);
                if (info1.ReturnValue == ReturnMessage.Success)
                {
                    int remain = BitConverter.ToInt32(info1.ValueData, 0);
                    string money = Convert.ToString(remain / 100);
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        txtMsg.AppendText("\r\n卡内剩余：" + money + " 块钱。");
                    }));
                }
                Thread.Sleep(300);
            }
        }

        /// <summary>
        /// 钱包初始化
        /// </summary>
        /// <param name="coin">第一次充入的钱，以元为单位</param>
        /// <param name="blockNum">用于初始化的绝对块号</param>
        /// <returns></returns>
        private async Task InitValue(int coin, byte blockNum)
        {
            // 选卡操作
            byte[] uid = await Select();
            if (uid == null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.Text = "没有选中任何卡片！";
                }));
                return;
            }
            //证实
            if (!await AuthKey((byte)(blockNum / 4)))
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.Text = "证实失败！";
                }));
                return;
            }
            //初始化钱包
            byte[] data = BitConverter.GetBytes(coin * 100);
            var info = await i14443a.InitValueAsync(blockNum, data);
            if (info.ReturnValue == ReturnMessage.Success)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.Text = "钱包初始化完毕，已充入 " + coin.ToString() + "元钱。";
                }));
            }
        }

        /// <summary>
        /// 选卡操作
        /// </summary>
        /// <returns>返回被选中的标签的 UID，如果未选中标签，则返回 null</returns>
        private async Task<byte[]> Select()
        {
            //请求操作
            var info = await i14443a.RequestAsync(RequestMode.AllCard);
            if (info.ReturnValue != ReturnMessage.Success)
            {
                return null;
            }
            //防冲突操作，感应区内只允许放置一张卡片
            var info1 = await i14443a.Anticoll2Async(MultiLable.AllowOne);
            if (info1.ReturnValue != ReturnMessage.Success)
            {
                return null;
            }
            //选卡操作
            var info2 = await i14443a.SelectAsync(info1.UID);
            if (info2.ReturnValue != ReturnMessage.Success)
            {
                return null;
            }
            return info1.UID;
        }

        /// <summary>
        /// 证实指定扇区
        /// </summary>
        /// <param name="sectorNum">要证实的扇区</param>
        /// <returns>证实成功则返回 true,否则返回 fasle。</returns>
        private async Task<bool> AuthKey(byte sectorNum)
        {
            byte[] keyA = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            //密码 A 证实
            var info3 = await i14443a.AuthKeyAsync(KeyType.KeyA, sectorNum, keyA);
            if (info3.ReturnValue != ReturnMessage.Success)
            {
                Console.WriteLine(info3.GetStatusStr());
                return false;
            }
            return true;
        }

        private bool CompareUID(byte[] uid1, byte[] uid2)
        {
            if (uid1 == null || uid2 == null) return false;
            if (uid1.Length != uid2.Length) return false;

            for (int i = 0; i < uid1.Length; i++)
            {
                if (uid1[i] != uid2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}