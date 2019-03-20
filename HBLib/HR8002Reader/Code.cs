using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.HR8002Reader
{
    /// <summary>
    /// 读写器自定义命令的操作命令符(Cmd)为0x00，
    /// 由操作控制符(State)决定具体执行哪条自定义命令。
    /// 此结构体表示 State 中的常量
    /// </summary>
    public struct ReaderState
    {
        /// <summary>
        /// 获得读写器地址、读写器软件版本号、
        /// 读写器类型代码、读写器协议支持信息和InventoryScanTime
        /// </summary>
        public const byte GetReaderInformation = 0x00;
        /// <summary>
        /// 关闭感应场。
        /// </summary>
        public const byte CloseRF = 0x01;
        /// <summary>
        /// 打开感应场。
        /// </summary>
        public const byte OpenRF = 0x02;
        /// <summary>
        /// 设置读写器地址
        /// </summary>
        public const byte WriteComAdr = 0x03;
        /// <summary>
        /// 设置读写器InventoryScanTime的值。
        /// </summary>
        public const byte WriteInventoryScanTime = 0x04;
        /// <summary>
        /// 设置读写器为ISO14443A协议模式
        /// </summary>
        public const byte ChangToISO14443A = 0x05;
        /// <summary>
        /// 设置读写器为ISO15693协议模式
        /// </summary>
        public const byte ChangToISO15693 = 0x06;
        /// <summary>
        /// 控制读写器LED状态
        /// </summary>
        public const byte SetLED = 0x07;
        /// <summary>
        /// 控制读写器蜂鸣器状态
        /// </summary>
        public const byte Beep = 0x08;
        /// <summary>
        /// 设置读写器为ISO14443B协议模式
        /// </summary>
        public const byte ChangToISO14443B = 0x09;        
    }
}
