using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJYZNLib;
using LJYZNLib.LJYZN105Reader;
using LJYZNLib.ISO180006C;

namespace LJYZNCmd
{
    class Program
    {
        static ComPort com = new ComPort("COM4", 57600, 1000);
        static Reader reader;
        static I180006C i180006C;

        static void Main(string[] args)
        {
            reader = new Reader(0x00, com);
            i180006C = new I180006C(0x00, com);
            com.Open();
            SetReadProtect_G2();


            Console.ReadLine();
        }

        static async Task Inventory()
        {
            var info = await i180006C.InventoryMultiAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetStatusStr());
            Console.WriteLine(info.Num);
            foreach (var e in info.EPCSet)
            {
                Console.Write(e.EPCLen + ":");
                Console.Write(e.GetEPCStr());
                Console.WriteLine();
            }
        }
        static async Task ReadCare()
        {
            var info = await i180006C.InventorySingleAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetStatusStr());
            Console.WriteLine("查询到的标签数量：" + info.Num);
            Console.WriteLine("标签EPC：" + info.EPCSet[0].GetEPCStr());

            var info1 = await i180006C.ReadCardAsync(info.EPCSet[0].EPCLen, info.EPCSet[0].EPC,
                MemoryArea.User, 0, 6, new byte[] { 0, 0, 0, 0 }, 0, 0);
            Console.WriteLine(info1.GetSendByteStr());
            Console.WriteLine(info1.GetRecvByteStr());
            Console.WriteLine(info1.GetStatusStr());
            Console.WriteLine("数据：" + info1.GetDataStr());
        }
        static async Task WriteCard()
        {
            var info = await i180006C.InventorySingleAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetStatusStr());
            Console.WriteLine("查询到的标签数量：" + info.Num);
            Console.WriteLine("标签EPC：" + info.EPCSet[0].GetEPCStr());

            var info1 = await i180006C.WriteCardAsync(3, info.EPCSet[0].EPCLen, info.EPCSet[0].EPC,
                MemoryArea.User, 0, new byte[] { 1, 2, 3, 4, 5, 6 },
                new byte[] { 0, 0, 0, 0 }, 0, 0);
            Console.WriteLine(info1.GetSendByteStr());
            Console.WriteLine(info1.GetRecvByteStr());
            Console.WriteLine(info1.GetStatusStr());
        }
        static async Task SetReadProtect_G2()
        {
            var info = await i180006C.InventorySingleAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetStatusStr());
            Console.WriteLine("查询到的标签数量：" + info.Num);
            Console.WriteLine("标签EPC：" + info.EPCSet[0].GetEPCStr());

            var info1 = await i180006C.LockUserBlockAsync(info.EPCSet[0].EPCLen, info.EPCSet[0].EPC,
                new byte[] { 0, 0, 0, 0 }, 0, 0, 0);
            Console.WriteLine(info1.GetSendByteStr());
            Console.WriteLine(info1.GetRecvByteStr());
            Console.WriteLine(info1.GetStatusStr());
        }
    }
}
