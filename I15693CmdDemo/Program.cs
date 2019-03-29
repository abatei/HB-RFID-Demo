using HBLib;
using HBLib.HR8002Reader;
using HBLib.ISO15693;
using System;
using System.Text;
using System.Threading.Tasks;

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
            GetStringFromCard(1);

            Console.ReadLine();
        }
        //参数blockNum表示从哪个数据块开始读
        private static async Task GetStringFromCard(byte blockNum)
        {
            await reader.ChangeToISO15693Async();
            var info = await i15693.InventoryWithoutAFIAsync();
            if (info.ReturnValue != ReturnMessage.Success)
            {
                Console.WriteLine("选卡失败！");
                return;
            }
            await i15693.SelectAsync(info.UID); //让询查到的标签进入选中状态
            //首先读取第一个数据块以获取整个字节数组的长度
            var info1 = await i15693.ReadSingleBlockAsync(I15693BlockLen.Four, blockNum);
            int len = info1.BlockData[0] + 1;
            //计算需要读取的块的个数
            int count = len / 4 + ((len % 4 == 0) ? 0 : 1);
            var info2 = await i15693.ReadMultipleBlockAsync(I15693BlockLen.Four, blockNum, (byte)count); //读数据
            byte[] data = info2.GetBlockData();
            string str = Encoding.Unicode.GetString(data, 1, info1.BlockData[0]);
            Console.WriteLine(str);
        }
    }
}
