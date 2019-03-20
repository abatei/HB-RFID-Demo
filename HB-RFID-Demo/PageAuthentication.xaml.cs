using HBLib;
using HBLib.HR8002Reader;
using HBLib.ISO14443A;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System;
using System.Text;

namespace HB_RFID_Demo
{
    /// <summary>
    /// PageAuthentication.xaml 的交互逻辑
    /// </summary>
    public partial class PageAuthentication : Page
    {
        public PageAuthentication()
        {
            InitializeComponent();          
            txtMsg.Text = "本模块仅操作 15 号扇区！\r\n\r\n";
            txtMsg.AppendText("本模块为危险操作模块，建议使用模拟器完成本实验。操作稍有不慎，就会导致 15 号扇区作废！切记在更改密码 A 后，不要忘记密码。密码 A 永远无法读出，如果忘记，将导致 15 号扇区作废！\r\n\r\n");
            txtMsg.AppendText("【读取15号扇区控制块数据】按钮可用于验证密码 A 的正确性。\r\n\r\n");
            txtMsg.AppendText("【将密码更改回默认值】按钮可用于将密码 A 和密码 B 更改为默认值，必须输入正确密码 A 方能成功执行。\r\n\r\n");
            txtMsg.AppendText("【检查标签密码】按钮可供教师在实验完成后使用，它会检查所有扇区密码是否都为默认值。");

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            com = MainWindow.com;
            reader = new Reader(0x00, com);
            i14443a = new I14443A(0x00, com);
        }

        ComPort com;
        Reader reader;
        I14443A i14443a;

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private async Task Select()
        {
            var info = await reader.ChangeToISO14443AAsync();
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text = "发送的字节：" + info.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info.GetRecvByteStr();
                txtMsg.Text += "\r\n" + info.GetStatusStr();
            }));
            if (info.ReturnValue != ReturnMessage.Success)
            {
                return;
            }

            var info1 = await i14443a.RequestAsync(RequestMode.IdleCard);
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text += "\r\n\r\n发送的字节：" + info1.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info1.GetRecvByteStr();
                txtMsg.Text += "\r\n卡片类型：" + info1.GetCardTypeName();
                txtMsg.Text += "\r\n" + info1.GetStatusStr();
            }));
            if (info1.ReturnValue != ReturnMessage.Success)
            {
                return;
            }

            var info2 = await i14443a.AnticollAsync();
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text += "\r\n\r\n发送的字节：" + info2.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info2.GetRecvByteStr();
                txtMsg.Text += "\r\n返回的标签 UID：" + info2.GetUIDStr();
                txtMsg.Text += "\r\n" + info2.GetStatusStr();
            }));
            if (info2.ReturnValue != ReturnMessage.Success)
            {
                return;
            }

            var info3 = await i14443a.SelectAsync(info2.UID);
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text += "\r\n\r\n发送的字节：" + info.GetSendByteStr();
                txtMsg.Text += "\r\n接收的字节：" + info.GetRecvByteStr();
                txtMsg.Text += "\r\n" + info.GetStatusStr();
            }));
            if (info3.ReturnValue != ReturnMessage.Success)
            {
                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                txtSelCard.Text = info2.GetUIDStr();
            }));
        }

        private void Btn_ChangePswA_Click(object sender, RoutedEventArgs e)
        {
            ChangePsw(KeyType.KeyA);
        }
        private void Btn_ChangePswB_Click(object sender, RoutedEventArgs e)
        {
            ChangePsw(KeyType.KeyB);
        }
        private async Task ChangePsw(KeyType keyType)
        {
            if (txtSelCard.Text == "")
            {
                MessageBox.Show("请先进行选卡操作！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (txtOldKey.Text == "" || txtNewKey.Text == "")
            {
                MessageBox.Show("请在密码A编辑框和新密码编辑框输入相应密码！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            byte[] keyA;
            if (txtOldKey.Text == "默认")
            {
                keyA = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            }
            else
            {
                keyA = new byte[6];
                byte[] b = Encoding.ASCII.GetBytes(txtOldKey.Text);
                if (b.Length > 6)
                {
                    MessageBox.Show("密码A编辑框输入的密码超出长度！", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                b.CopyTo(keyA, 0);
            }

            var info = await i14443a.AuthKeyAsync(KeyType.KeyA, 15, keyA);
            this.Dispatcher.Invoke(new Action(() =>
            {

                txtMsg.Text = "发送的字节：" + info.GetSendByteStr();
                txtMsg.AppendText("\r\n接收的字节：" + info.GetRecvByteStr());
                txtMsg.AppendText("\r\n" + info.GetStatusStr());
            }));
            if (info.ReturnValue != ReturnMessage.Success) return;

            var info1 = await i14443a.ReadAsync(63);
            this.Dispatcher.Invoke(new Action(() =>
            {

                txtMsg.AppendText("\r\n\r\n发送的字节：" + info1.GetSendByteStr());
                txtMsg.AppendText("\r\n接收的字节：" + info1.GetRecvByteStr());
                txtMsg.AppendText("\r\n读取结果：" + info1.GetBlockDataStr());
                txtMsg.AppendText("\r\n" + info1.GetStatusStr());
            }));
            if (info1.ReturnValue != ReturnMessage.Success) return;

            byte[] newKey = new byte[6];
            byte[] b1 = Encoding.ASCII.GetBytes(txtNewKey.Text);
            if (b1.Length > 6)
            {
                MessageBox.Show("新密码超出长度！", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            b1.CopyTo(newKey, 0);

            byte[] newData = info1.BlockData;
            if (keyType == KeyType.KeyA)
            {
                newKey.CopyTo(newData, 0);
            }
            else
            {
                keyA.CopyTo(newData, 0);
                newKey.CopyTo(newData, 10);
            }

            var info2 = await i14443a.WriteAsync(63, newData);
            this.Dispatcher.Invoke(new Action(() =>
            {

                txtMsg.AppendText("\r\n\r\n发送的字节：" + info2.GetSendByteStr());
                txtMsg.AppendText("\r\n接收的字节：" + info2.GetRecvByteStr());
                txtMsg.AppendText("\r\n" + info2.GetStatusStr());
            }));
            if (info2.ReturnValue == ReturnMessage.Success)
            {
                if (keyType == KeyType.KeyA)
                {
                    txtOldKey.Text = txtNewKey.Text;
                }
            }
        }

        private void Btn_Read_Click(object sender, RoutedEventArgs e)
        {
            Read();
        }
        private async Task Read()
        {
            if (txtSelCard.Text == "")
            {
                MessageBox.Show("请先进行选卡操作！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (txtOldKey.Text == "")
            {
                MessageBox.Show("请在密码A编辑框输入密码A！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            byte[] keyA;
            if (txtOldKey.Text == "默认")
            {
                keyA = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            }
            else
            {
                keyA = new byte[6];
                byte[] b = Encoding.ASCII.GetBytes(txtOldKey.Text);
                if (b.Length > 6)
                {
                    MessageBox.Show("密码A编辑框输入的密码超出长度！", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                b.CopyTo(keyA, 0);
            }

            var info = await i14443a.AuthKeyAsync(KeyType.KeyA, 15, keyA);
            this.Dispatcher.Invoke(new Action(() =>
            {

                txtMsg.Text = "发送的字节：" + info.GetSendByteStr();
                txtMsg.AppendText("\r\n接收的字节：" + info.GetRecvByteStr());
                txtMsg.AppendText("\r\n" + info.GetStatusStr());
            }));
            if (info.ReturnValue != ReturnMessage.Success) return;

            var info1 = await i14443a.ReadAsync(63);
            this.Dispatcher.Invoke(new Action(() =>
            {

                txtMsg.AppendText("\r\n\r\n发送的字节：" + info1.GetSendByteStr());
                txtMsg.AppendText("\r\n接收的字节：" + info1.GetRecvByteStr());
                txtMsg.AppendText("\r\n读取结果：" + info1.GetBlockDataStr());
                txtMsg.AppendText("\r\n" + info1.GetStatusStr());
            }));
        }

        private void Btn_DefaultKey_Click(object sender, RoutedEventArgs e)
        {
            ChangeToDefaultPsw();
        }
        private async Task ChangeToDefaultPsw()
        {
            if (txtSelCard.Text == "")
            {
                MessageBox.Show("请先进行选卡操作！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (txtOldKey.Text == "")
            {
                MessageBox.Show("请在密码A编辑框输入密码 A！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            byte[] keyA;
            if (txtOldKey.Text == "默认")
            {
                keyA = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            }
            else
            {
                keyA = new byte[6];
                byte[] b = Encoding.ASCII.GetBytes(txtOldKey.Text);
                if (b.Length > 6)
                {
                    MessageBox.Show("密码A编辑框输入的密码超出长度！", "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                b.CopyTo(keyA, 0);
            }
            var info = await i14443a.AuthKeyAsync(KeyType.KeyA, 15, keyA);
            this.Dispatcher.Invoke(new Action(() =>
            {

                txtMsg.Text = "发送的字节：" + info.GetSendByteStr();
                txtMsg.AppendText("\r\n接收的字节：" + info.GetRecvByteStr());
                txtMsg.AppendText("\r\n" + info.GetStatusStr());
            }));
            if (info.ReturnValue != ReturnMessage.Success) return;

            byte[] data = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x07, 0x80, 0x69, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            var info2 = await i14443a.WriteAsync(63, data);
            this.Dispatcher.Invoke(new Action(() =>
            {

                txtMsg.AppendText("\r\n\r\n发送的字节：" + info2.GetSendByteStr());
                txtMsg.AppendText("\r\n接收的字节：" + info2.GetRecvByteStr());
                txtMsg.AppendText("\r\n" + info2.GetStatusStr());
            }));
            if (info2.ReturnValue == ReturnMessage.Success)
            {
                txtOldKey.Text = "默认";
            }
        }

        private void Btn_Check_Click(object sender, RoutedEventArgs e)
        {
            CheckPsw();
        }
        private async Task CheckPsw()
        {
            if (txtSelCard.Text == "")
            {
                MessageBox.Show("请先进行选卡操作！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (txtOldKey.Text == "")
            {
                MessageBox.Show("请在密码A编辑框输入密码 A！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            byte[] keyA={ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            StringBuilder result = new StringBuilder();
            for (byte sectorNum = 0; sectorNum < 16; sectorNum++)
            {
                var info = await i14443a.AuthKeyAsync(KeyType.KeyA, sectorNum, keyA);
                if (info.ReturnValue != ReturnMessage.Success)
                {
                    if(result.Length!=0)
                    {
                        result.Append("\r\n");
                    }
                    result.Append("扇区");
                    result.Append(sectorNum.ToString());
                    result.Append(" 验证失败！");
                }
            }
            if(result.Length==0)
            {
                result.Append("所有扇区的密码 A 均为默认值！");
            }
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtMsg.Text = result.ToString();
            }));
        }
    }
}
