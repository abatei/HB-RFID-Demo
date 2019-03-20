using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib.LJYZN105Reader
{
    /// <summary>
    /// LJYZN105 读写器自定义命令
    /// </summary>
    public struct ReaderCmd
    {
        /// <summary>
        /// 读取读写器信息
        /// </summary>
        public const byte GetInformation = 0x21;
        /// <summary>
        /// 设置读写器工作频率
        /// </summary>
        public const byte SetFrequency = 0x22;
        /// <summary>
        /// 设置读写器地址
        /// </summary>
        public const byte SetAddr = 0x24;
        /// <summary>
        /// 设置读写器询查时间
        /// </summary>
        public const byte SetInventoryTime = 0x25;
        /// <summary>
        /// 设置读写器的波特率
        /// </summary>
        public const byte SetBaudRate = 0x28;
        /// <summary>
        /// 调整读写器输出功率
        /// </summary>
        public const byte SetPowerDbm = 0x2F;
        /// <summary>
        /// 声光控制命令
        /// </summary>
        public const byte BuzzerAndLEDControl = 0x33;
    }
}
