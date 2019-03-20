using System;
using System.Text;

namespace LJYZNLib
{
    public class FrameBase
    {
        byte[] dataBlock; //整个数据帧
        private byte len; //数据帧长度
        private byte com_adr = 0; //读写器地址

        public byte[] DataBlock
        {
            get { return dataBlock; }
        }

        public FrameBase() { }
        /// <summary>
        /// 当主机地址为默认值 0x0000，读写器地址为默认值 0x0001 时调用此构造函数
        /// </summary>
        /// <param name="len">报文长度</param>
        public FrameBase(byte len) : this(len, 0x00) { }

        /// <summary>
        /// 构造函数，当自定义主机及读写器地址时使用
        /// </summary>
        /// <param name="len">报文长度，最大值为96</param>
        /// <param name="com_adr">读写器地址</param>
        public FrameBase(byte len, byte com_adr)
        {
            dataBlock = new byte[len + 1];
            //在数据帧压入Len;
            this.len = len;
            dataBlock[0] = len;
            //设置读写器地址Com_adr
            this.com_adr = com_adr;
            dataBlock[1] = com_adr;
        }

        public void PushCRC()
        {
            UInt16 CRC = GetCRC(dataBlock);
            byte[] crcByte = BitConverter.GetBytes(CRC);
            crcByte.CopyTo(dataBlock, dataBlock.Length - 2);
        }

        public static UInt16 GetCRC(byte[] block)
        {
            UInt16 polynomial = 0x8408;
            UInt16 CRC = 0xFFFF;
            for (int i = 0; i < block.Length - 2; i++)
            {
                CRC ^= block[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((CRC & 0x0001) != 0)
                    {
                        CRC = (UInt16)((CRC >> 1) ^ polynomial);
                    }
                    else
                    {
                        CRC >>= 1;
                    }
                }
            }
            return CRC;
        }
        public static byte[] NumReverseToByte(UInt16 num)
        {
            byte[] b = BitConverter.GetBytes(num);
            byte temp = b[0];
            b[0] = b[1];
            b[1] = temp;
            return b;
        }
        public static UInt16 Reverse(UInt16 num)
        {
            return (UInt16)(((num & 0xFF00) >> 8) | ((num & 0x00FF) << 8));
        }
        public static string ByteSetToString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        public static string UIDToString(byte[] uid)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in uid)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
