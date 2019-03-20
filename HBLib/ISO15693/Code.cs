using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.ISO15693
{
    /// <summary>
    /// 不带AFI的Inventory-scan命令中的 State 值
    /// </summary>
    public enum InventoryScanWithoutAFIMode : byte
    {
        /// <summary>
        /// 不带AFI的Inventory-scan的新询查
        /// </summary>
        NewScanWithoutAFI = 0x06,
        /// <summary>
        /// 不带AFI的Inventory-scan的继续询查
        /// </summary>
        ContinueScanWithoutAFI = 0x02
    }

    /// <summary>
    /// 带AFI的Inventory-scan命令中的 State 值
    /// </summary>
    public enum InventoryScanWithAFIMode : byte
    {
        /// <summary>
        /// 带AFI的Inventory-scan的新询查
        /// </summary>
        NewScanWithAFI = 0x07,
        /// <summary>
        /// 带AFI的Inventory-scan的继续询查
        /// </summary>
        ContinueScanWithAFI = 0x03
    }

    public enum I15693CardType : byte
    {
        TypeA,
        TypeB
    }

    public enum I15693BlockLen : byte
    {
        Four = 4,
        Eight = 8
    }

    public struct I15693Cmd
    {
        /// <summary>
        /// 询查命令，检查有效范围内是否有标签的存在
        /// </summary>
        public const byte Inventory = 0x01;
        /// <summary>
        /// 让指定的电子标签进入Quiet状态，对inventory命令不响应
        /// </summary>
        public const byte StayQuiet = 0x02;
        /// <summary>
        /// 将指定的块的数据读出来，每个块有4或8个字节的数据，并包含这个块的安全状态信息
        /// </summary>
        public const byte ReadSingleBlock = 0x20;
        /// <summary>
        /// 将长度为4或8个字节的数据写入指定的块
        /// </summary>
        public const byte WriteSingleBlock = 0x21;
        /// <summary>
        /// 锁定指定的块，使之“写保护”
        /// </summary>
        public const byte LockBlock = 0x22;
        /// <summary>
        /// 将指定的多个块的数据一次性读出来，每个块有4或8个字节的数据，并包含这个块的安全状态信息
        /// </summary>
        public const byte ReadMultipleBlock = 0x23;
        /// <summary>
        /// 让选定的电子标签进入“selected状态”
        /// </summary>
        public const byte Select = 0x25;
        /// <summary>
        /// 让电子标签回到Ready状态
        /// </summary>
        public const byte ResetToReady = 0x26;
        /// <summary>
        /// 向指定的电子标签写入应用类型识别码
        /// </summary>
        public const byte WriteAFI = 0x27;
        /// <summary>
        /// 锁定指定的电子标签的应用类型识别码
        /// </summary>
        public const byte LockAFI = 0x28;
        /// <summary>
        /// 向指定的电子标签写入数据保存格式识别码
        /// </summary>
        public const byte WriteDSFID = 0x29;
        /// <summary>
        /// 锁定指定的电子标签的数据保存格式识别码
        /// </summary>
        public const byte LockDSFID = 0x2A;
        /// <summary>
        /// 获得指定电子标签的详细信息
        /// </summary>
        public const byte GetSystemInformation = 0x2B;
    }
}
