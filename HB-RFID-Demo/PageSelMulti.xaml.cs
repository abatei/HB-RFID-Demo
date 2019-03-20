using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using HBLib;
using HBLib.HR8002Reader;
using HBLib.ISO14443A;

namespace HB_RFID_Demo
{
    /// <summary>
    /// PageSelMulti.xaml 的交互逻辑
    /// </summary>
    public partial class PageSelMulti : Page
    {
        public PageSelMulti()
        {
            InitializeComponent();
            com = MainWindow.com;
            reader = new Reader(0x00, com);
            i14443a = new I14443A(0x00, com);
        }

        ComPort com;
        Reader reader;
        I14443A i14443a;
        byte[] anticollCard; //防冲突返回的标签UID
        byte[] selCardID; //当前选中标签的UID

        private void BtnChangeTo14443A_Click(object sender, RoutedEventArgs e)
        {
            ChangeTo14443A();
        }
        private async Task ChangeTo14443A()
        {
            var info = await reader.ChangeToISO14443AAsync();
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text = "发送的字节：" + info.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info.GetRecvByteStr();
                txtMsg.Text += "\r\n" + info.GetStatusStr();
            }));
        }

        private void BtnRequest_Click(object sender, RoutedEventArgs e)
        {
            Request();
        }
        private async Task Request()
        {
            i14443a = new I14443A(0x00, com);
            var info = await i14443a.RequestAsync(RequestMode.IdleCard);
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text = "发送的字节：" + info.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info.GetRecvByteStr();
                txtMsg.Text += "\r\n卡片类型：" + info.GetCardTypeName();
                txtMsg.Text += "\r\n" + info.GetStatusStr();
            }));
        }

        private void BtnAnticoll_Click(object sender, RoutedEventArgs e)
        {
            Anticoll();
        }
        private async Task Anticoll()
        {
            var info = await i14443a.AnticollAsync();
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text = "发送的字节：" + info.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info.GetRecvByteStr();
                txtMsg.Text += "\r\n返回的标签 UID：" + info.GetUIDStr();
                txtMsg.Text += "\r\n" + info.GetStatusStr();
            }));
            if (info.ReturnValue == ReturnMessage.Success)
            {
                anticollCard = info.UID;
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectCard();
        }
        private async Task SelectCard()
        {
            if (anticollCard == null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.Text = "请首先执行请求、防冲突命令后再选择标签！";
                }));
                return;
            }
            var info = await i14443a.SelectAsync(anticollCard);
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text = "发送的字节：" + info.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info.GetRecvByteStr();
                txtMsg.Text += "\r\n" + info.GetStatusStr();
            }));
            if(info.ReturnValue==ReturnMessage.Success)
            {
                selCardID = anticollCard;
                string uid = FrameBase.UIDToString(selCardID);
                txtCard.Text = uid;
                if (!lstCard.Items.Contains(uid))
                {
                    lstCard.Items.Add(uid);
                }
            }
        }

        private void BtnHalt_Click(object sender, RoutedEventArgs e)
        {
            Halt();
        }
        private async Task Halt()
        {
            if(selCardID==null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtMsg.Text = "请先选择一张标签！";
                }));
                return;
            }
            var info = await i14443a.HaltAsync();
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text = "\r\n发送的字节：" + info.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info.GetRecvByteStr();
                txtMsg.Text += "\r\n" + info.GetStatusStr();               
            }));
            if (info.ReturnValue == ReturnMessage.Success)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtCard.Text = "";
                }));
                anticollCard = null;
                selCardID = null;
            }
        }
    }
}
