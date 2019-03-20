using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HBLib;
using HBLib.HR8002Reader;
using HBLib.ISO15693;

namespace I15693CmdDemo
{
    class Program
    {
        static ComPort com = new ComPort("COM3", 19200, 3000);
        static Reader reader;
        static I15693 i15693;

        static void Main(string[] args)
        {
            com.Open();
            reader = new Reader(0x00, com);
            i15693 = new I15693(0x00, com);
            //WriteSingleBlock();
            //ReadMultipleBlock();
            GetSystemInformation();

            Console.ReadLine();
        }

        private static async Task InventoryWithoutAFI()
        {
            await reader.ChangeToISO15693Async();

            var info = await i15693.InventoryWithoutAFIAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetUIDStr());
            Console.WriteLine(info.DSFID);
            Console.WriteLine(info.GetStatusStr());
        }

        private static async Task InventoryWithAFI(byte afi)
        {
            await reader.ChangeToISO15693Async();

            var info = await i15693.InventoryWithAFIAsync(afi);
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetUIDStr());
            Console.WriteLine(info.DSFID);
            Console.WriteLine(info.GetStatusStr());
        }

        private static async Task InventoryScanWithAFI(InventoryScanWithAFIMode mode, byte afi)
        {
            await reader.ChangeToISO15693Async();

            var info = await i15693.InventoryScanWithAFIAsync(mode, afi);
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            foreach (var card in info.CardSet)
            {
                Console.Write("\r\nDSFID：" + card.DSFID + "  UID：" + card.GetUIDStr());
            }
            Console.WriteLine();
            Console.WriteLine(info.GetStatusStr());
        }

        private static async Task StayQuietAsync()
        {
            await reader.ChangeToISO15693Async();
            var info = await i15693.InventoryWithoutAFIAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetUIDStr());
            Console.WriteLine(info.DSFID);
            Console.WriteLine(info.GetStatusStr());
            Console.WriteLine();

            var info1 = await i15693.StayQuietAsync(info.UID);
            Console.WriteLine(info1.GetSendByteStr());
            Console.WriteLine(info1.GetRecvByteStr());
            Console.WriteLine(info1.GetStatusStr());
        }

        private static async Task ReadSingleBlock()
        {
            await reader.ChangeToISO15693Async();
            var info = await i15693.InventoryWithoutAFIAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetUIDStr());
            Console.WriteLine(info.DSFID);
            Console.WriteLine(info.GetStatusStr());
            Console.WriteLine();

            var info1 = await i15693.ReadSingleBlockAsync(I15693BlockLen.Four, info.UID, 1);
            Console.WriteLine(info1.GetSendByteStr());
            Console.WriteLine(info1.GetRecvByteStr());
            Console.WriteLine(info1.BlockSecurityStatus);
            Console.WriteLine(info1.GetBlockDataStr());
            Console.WriteLine(info1.GetStatusStr());
        }

        private static async Task WriteSingleBlock()
        {
            await reader.ChangeToISO15693Async();
            var info = await i15693.InventoryWithoutAFIAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetUIDStr());
            Console.WriteLine(info.DSFID);
            Console.WriteLine(info.GetStatusStr());
            Console.WriteLine();

            byte[] data = { 1, 2, 3, 4 };
            var info1 = await i15693.WriteSingleBlockAsync(I15693CardType.TypeB, I15693BlockLen.Four, info.UID, 2, data);
            Console.WriteLine(info1.GetSendByteStr());
            Console.WriteLine(info1.GetRecvByteStr());
            Console.WriteLine(info1.GetStatusStr());
        }

        private static async Task ReadMultipleBlock()
        {
            await reader.ChangeToISO15693Async();
            var info = await i15693.InventoryWithoutAFIAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetUIDStr());
            Console.WriteLine(info.DSFID);
            Console.WriteLine(info.GetStatusStr());
            Console.WriteLine();

            var info1 = await i15693.ReadMultipleBlockAsync(I15693BlockLen.Four, info.UID, 1, 3);
            Console.WriteLine(info1.GetSendByteStr());
            Console.WriteLine(info1.GetRecvByteStr());
            Console.WriteLine(info1.GetBlockDataStr());
            Console.WriteLine(info1.GetStatusStr());
        }

        private static async Task GetSystemInformation()
        {
            await reader.ChangeToISO15693Async();

            var info = await i15693.InventoryWithoutAFIAsync();
            Console.WriteLine(info.GetSendByteStr());
            Console.WriteLine(info.GetRecvByteStr());
            Console.WriteLine(info.GetUIDStr());
            Console.WriteLine(info.DSFID);
            Console.WriteLine(info.GetStatusStr());

            var info1 = await i15693.GetSystemInformationAsync(info.UID);
            Console.WriteLine(info1.GetSendByteStr());
            Console.WriteLine(info1.GetRecvByteStr());
            Console.WriteLine(info1.GetStatusStr());
            Console.WriteLine("UID：" + info1.GetUIDStr());
            Console.WriteLine("DSFID：" + info1.DSFID);
            Console.WriteLine("AFI：" + info1.AFI);
            Console.WriteLine("MemorySize：" + FrameBase.ByteSetToString(info1.MemorySize));
            Console.WriteLine("ICReference：" + info1.ICReference);
        }
    }
}
