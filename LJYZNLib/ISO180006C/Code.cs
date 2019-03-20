using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib.ISO180006C
{
    public struct I180006CCmd
    {
        /// <summary>
        /// 询查多标签
        /// </summary>
        public const byte InventoryMulti = 0x01;
        /// <summary>
        /// 读数据
        /// </summary>
        public const byte ReadCard = 0x02;
        /// <summary>
        /// 写数据
        /// </summary>
        public const byte WriteCard = 0x03;
        /// <summary>
        /// 写EPC号
        /// </summary>
        public const byte WriteEPC = 0x04;
        /// <summary>
        /// 销毁标签
        /// </summary>
        public const byte DestroyCard = 0x05;
        /// <summary>
        /// 设定存储区读写保护状态
        /// </summary>
        public const byte SetCardProtect = 0x06;
        /// <summary>
        /// 块擦除
        /// </summary>
        public const byte EraseCard = 0x07;
        /// <summary>
        /// 根据EPC号设定读保护设置
        /// </summary>
        public const byte SetReadProtect_G2 = 0x08;
        /// <summary>
        /// 不需要EPC号读保护设定
        /// </summary>
        public const byte SetMultiReadProtect = 0x09;
        /// <summary>
        /// 解锁读保护
        /// </summary>
        public const byte RemoveReadProtect = 0x0A;
        /// <summary>
        /// 测试标签是否被设置读保护
        /// </summary>
        public const byte CheckReadProtected = 0x0B;
        /// <summary>
        /// EAS报警设置
        /// </summary>
        public const byte SetEASAlarm = 0x0C;
        /// <summary>
        /// EAS报警探测
        /// </summary>
        public const byte CheckEASAlarm = 0x0D;
        /// <summary>
        /// user区块锁
        /// </summary>
        public const byte LockUserBlock = 0x0E;
        /// <summary>
        /// 询查单标签
        /// </summary>
        public const byte InventorySingle = 0x0F;
    }

    /// <summary>
    /// 标签存储区
    /// </summary>
    public enum MemoryArea : byte
    {
        /// <summary>
        /// 保留区
        /// </summary>
        Reserve=0x00,
        /// <summary>
        /// EPC存储区
        /// </summary>
        EPC = 0x01,
        /// <summary>
        /// TID存储区
        /// </summary>
        TID = 0x02,
        /// <summary>
        /// 用户存储区
        /// </summary>
        User = 0x03
    }

    /// <summary>
    /// 设定存储区读写保护状态（SetCardProtect）时用于 Select 字段，用于设定保护的区
    /// </summary>
    public enum ProtectArea:byte
    {
        /// <summary>
        /// 控制Kill密码读写保护设定
        /// </summary>
        KillPassword = 0x00,
        /// <summary>
        /// 控制访问密码读写保护设定
        /// </summary>
        AccessPassword = 0x01,
        /// <summary>
        /// 控制EPC存储区读写保护设定
        /// </summary>
        EPCArea = 0x02,
        /// <summary>
        /// 控制TID存储区读写保护设定
        /// </summary>
        TIDArea = 0x03,
        /// <summary>
        /// 用户存储区读写保护设定
        /// </summary>
        UserArea = 0x04
    }
}
