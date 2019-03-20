using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HBLib;
using HBLib.HR8002Reader;
using HBLib.ISO14443A;

namespace CmdReader
{
    class Program
    {
        static ComPort com = new ComPort("COM3", 19200, 300);

        static void Main(string[] args)
        {
            com.Open();
            LoadKey();

            Console.ReadLine();
        }

        private static async Task LoadKey()
        {
            Reader reader = new Reader(0x00, com);
            I14443A i14443a = new I14443A(0x00, com);
            //更改为 ISO14443A 模式
            await reader.ChangeToISO14443AAsync();
            //将密码存入 EEPROM
            byte[] keyB = Encoding.ASCII.GetBytes("123456");
            var info = await i14443a.LoadKeyAsync(KeyType.KeyB, 15, keyB);
            Console.WriteLine("发送的字节：" + info.GetSendByteStr());
            Console.WriteLine("接收的字节：" + info.GetRecvByteStr());
            Console.WriteLine(info.GetStatusStr());
        }
    }
}
