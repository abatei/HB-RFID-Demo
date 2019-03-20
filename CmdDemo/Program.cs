using System;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using HBLib;
using HBLib.HR8002Reader;
using HBLib.ISO14443A;
using Newtonsoft.Json;

namespace CmdDemo
{
    class Program
    {
        static ComPort com = new ComPort("COM6", 19200, 1000);
        static Reader reader;
        static I14443A i14443a;

        static void Main(string[] args)
        {
            com.Open();
            reader = new Reader(0x00, com);
            i14443a = new I14443A(0x00, com);

            InitValue(1, 502);

            Console.ReadLine();
        }


        //向指定扇区写入指定数据
        private static async Task InitValue(byte blockNum, int value)
        {
            await AuthKey(0);
            //await reader.ChangeToISO14443AAsync();
            byte[] data = { 0, 0, 0, 1 };
            var info = await i14443a.ValueAsync(OperCode.Increase, 4, data, 5);
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetStatusStr());






            //if (!await AuthKey((byte)(blockNum / 4))) return;
            //byte[] data = BitConverter.GetBytes(value);
            //var info = await i14443a.InitValueAsync(blockNum, data);
            //Console.WriteLine(info.GetSendByteStr());
            //Console.WriteLine(info.GetRecvByteStr());
            //if (info.ReturnValue == ReturnMessage.Success)
            //{
            //    Console.WriteLine("值块" + blockNum.ToString() + "初始化成功");
            //}
            //else
            //{
            //    Console.WriteLine(info.GetStatusStr());
            //}
        }

        //选卡
        private static async Task<bool> AuthKey(byte sectorNum)
        {

            await reader.ChangeToISO14443AAsync();
            //请求操作
            var info = await i14443a.RequestAsync(RequestMode.AllCard);
            if (info.ReturnValue != ReturnMessage.Success)
            {
                Console.WriteLine(info.GetStatusStr());
                return false;
            }
            //防冲突操作
            var info1 = await i14443a.AnticollAsync();
            if (info1.ReturnValue != ReturnMessage.Success)
            {
                Console.WriteLine(info1.GetStatusStr());
                return false;
            }
            //选卡操作
            var info2 = await i14443a.SelectAsync(info1.UID);
            if (info2.ReturnValue == ReturnMessage.Success)
            {
                Console.WriteLine("选中标签：" + info1.GetUIDStr());
            }
            else
            {
                Console.WriteLine(info2.GetStatusStr());
                return false;
            }

            //证实
            byte[] keyA = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            //密码 A 证实
            var info3 = await i14443a.AuthKeyAsync(KeyType.KeyA, sectorNum, keyA);
            if (info3.ReturnValue != ReturnMessage.Success)
            {
                Console.WriteLine(info3.GetStatusStr());
                return false;
            }
            Console.WriteLine("证实成功");
            return true;
        }
    }
}