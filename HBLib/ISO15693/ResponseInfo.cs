using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.ISO15693
{
    public class InventoryInfo : InfoBase
    {
        public byte DSFID { get; set; }
        public byte[] UID { get; set; } = new byte[8];

        public InventoryInfo() { }
        public InventoryInfo(byte[] frame) : base(frame) { }

        /// <summary>
        /// 返回电子标签序列号的字符串形式
        /// </summary>
        /// <returns></returns>
        public string GetUIDStr()
        {
            return FrameBase.UIDToString(UID);
        }
    }

    public class InventoryScanInfo : InfoBase
    {
        public class CardInfo
        {
            public byte DSFID { get; set; }
            public byte[] UID { get; set; } = new byte[8];

            public string GetUIDStr()
            {
                return FrameBase.UIDToString(UID);
            }
        }

        public List<CardInfo> CardSet { get; set; } = new List<CardInfo>();

        public InventoryScanInfo() { }
        public InventoryScanInfo(byte[] frame) : base(frame) { }
    }

    public class ReadSingleBlockInfo : InfoBase
    {
        public byte BlockSecurityStatus { get; set; } //块安全状态
        public byte[] BlockData { get; set; }

        public ReadSingleBlockInfo() { }
        public ReadSingleBlockInfo(I15693BlockLen len, byte[] frame) : base(frame)
        {
            if (len == I15693BlockLen.Four)
            {
                BlockData = new byte[4];
            }
            else
            {
                BlockData = new byte[8];
            }
        }

        public string GetBlockDataStr()
        {
            return FrameBase.ByteSetToString(BlockData);
        }
    }

    public class ReadMultipleBlockInfo : InfoBase
    {
        public byte[] Data { get; set; }
        private int blockLen;

        public ReadMultipleBlockInfo() { }
        public ReadMultipleBlockInfo(I15693BlockLen len, byte[] frame) : base(frame)
        {
            blockLen = (int)len;
            int dataLen = frame.Length - 5;
            Data = new byte[dataLen];
        }

        public string GetDataStr()
        {
            return FrameBase.ByteSetToString(Data);
        }

        public byte[] GetBlockData()
        {
            int count = Data.Length / (blockLen + 1); //读取的块的个数
            int len = count * blockLen; //不带块安全状态的数据长度
            byte[] blockData = new byte[len];
            int index = 1;
            int destIndex = 0;
            while (index < Data.Length)
            {
                Array.Copy(Data, index, blockData, destIndex, blockLen);
                index += blockLen + 1;
                destIndex += blockLen;
            }
            return blockData;
        }

        public string GetBlockDataStr()
        {
            byte[] blockData = GetBlockData();
            return FrameBase.ByteSetToString(blockData);
        }
    }

    public class GetSystemInformationInfo : InfoBase
    {
        public byte InformationFlag { get; set; }
        public byte[] UID { get; set; } = new byte[8];
        public byte DSFID { get; set; }
        public byte AFI { get; set; }
        public byte[] MemorySize { get; set; } = new byte[2]; //第一个字节表示标签最大块号，第二个字节表示每块最大字节编号
        public byte ICReference { get; set; }

        public GetSystemInformationInfo() { }
        public GetSystemInformationInfo(byte[] frame) : base(frame) { }

        public string GetUIDStr()
        {
            return FrameBase.UIDToString(UID);
        }
        /// <summary>
        /// 获取标签最大块号
        /// </summary>
        public int GetMaxBlockNum()
        {
            return MemorySize[0];
        }
        /// <summary>
        /// 获取卡的数据块所占空间的字节数
        /// </summary>
        /// <returns></returns>
        public I15693BlockLen GetBlockLen()
        {
            if(MemorySize[1]==0x03)
            {
                return I15693BlockLen.Four;
            }
            else
            {
                return I15693BlockLen.Eight;
            }
        }
    }
}
