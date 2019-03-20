using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib
{
    /// <summary>
    /// 常量状态码（Status）解释字符串
    /// </summary>
    public static class CodeInterpret
    {
        public static SortedList<ReturnMessage, string> ReturnMessageSet = new SortedList<ReturnMessage, string>()
        {
            [ReturnMessage.Success] = "操作成功！",
            [ReturnMessage.HF_FrameLenError] = "帧长度错误！",
            [ReturnMessage.HF_ReaderAddrError] = "读写器地址不匹配！",
            [ReturnMessage.HF_CRCError] = "接收消息的CRC效验错误！",
            [ReturnMessage.HF_StatusError] = "错误请参考状态码",
            [ReturnMessage.CP_OpenPortFailed] = "通信端口打开出错！",
            [ReturnMessage.CP_PortHasClosed] = "通信端口已经关闭！",
            [ReturnMessage.CP_TimeOutError] = "向读写器发送消息后，等待返回的消息超时！",
            [ReturnMessage.CP_ReadFaild] = "接收消息出错！",
            [ReturnMessage.CP_WriteFaild] = "发送消息出错"
        };

        public static SortedList<byte, string> StatusSet = new SortedList<byte, string>()
        {
            [0x00] = "操作成功！",
            [0x01] = "命令数据块中的命令操作数长度不符合！",
            [0x02] = "命令数据块的操作命令不被读写器支持！",
            [0x03] = "命令数据块中的命令操作数的某些字节数值不在允许的范围之内！",
            [0x04] = "操作命令当前无法执行！",
            [0x05] = "感应场处于关闭状态！",
            [0x06] = "向读写器内部EEPROM中写入数据失败！",
            [0x0A] = "操作ISO15693协议时，在指定的时间InventoryScanTime溢出前判断出感应场内可能有标签存在，但是还没有获得任何一张电子标签的UID！",
            [0x0B] = "操作ISO15693协议时，在指定的时间InventoryScanTime溢出前获得了部分但未获得全部感应场内的电子标签的UID！",
            [0x0C] = "读写器执行ISO15693协议命令的过程中出现了不符合协议规定的标签响应动作！",
            [0x0E] = "感应场内没有处于活动状态的电子标签可供操作！",
            [0x0F] = "ISO15693协议操作出错",
            [0x10] = "ISO14443A协议操作出错",
            [0x1B] = "ISO14443B协议操作出错",
            [0x11] = "SRI512/SRI4K命令执行出错",
            [0x1F] = "读写器模式错误！"
        };

        public static SortedList<byte, string> I14443AErrorSet = new SortedList<byte, string>()
        {
            [0x1F] = "Halt失败。",
            [0x20] = "有效区内无ISO/IEC14443A协议电子标签。",
            [0x21] = "选择电子标签失败。",
            [0x22] = "验证失败。",
            [0x23] = "读数据失败。",
            [0x24] = "写数据失败。",
            [0x25] = "钱包初始化失败。",
            [0x26] = "读钱包数据失败。",
            [0x27] = "钱包增减操作失败。",
            [0x28] = "数据存储传送失败。",
            [0x29] = "读写E2PROM失败。",
            [0x2A] = "存储密钥失败。",
            [0x2B] = "检查写失败。",
            [0x2C] = "检查写数据不符。",
            [0x2D] = "值操作失败。",
            [0x2E] = "Ultralight电子标签写失败。",
            [0x30] = "防冲突失败。",
            [0x31] = "不允多张电子标签进入。",
            [0x32] = "Mf1与Ultralight电子标签冲突。",
            [0x33] = "Ultralight电子标签冲突。"
        };

        public static SortedList<byte, string> I15693ErrorSet = new SortedList<byte, string>()
        {
            [0x01] = "命令不被支持。",
            [0x02] = "命令不被识别。",
            [0x03] = "该操作不被支持。",
            [0x0f] = "未知的错误类型。",
            [0x10] = "所指定的操作块不能被使用或不存在。",
            [0x11] = "所指定的操作块已经被锁定，不能再次被锁定。",
            [0x12] = "所指定的操作块已经被锁定，不能对其内容进行改写。",
            [0x13] = "所指定的操作块不能被正常的操作。",
            [0x14] = "所指定的操作块不能被正常的锁定。",
        };

        public static SortedList<byte, string> I14443BErrorSet = new SortedList<byte, string>()
        {
            [0x34] = "有效区内无ISO/IEC14443B协议电子标签。",
            [0x35] = "选择电子标签失败。",
            [0x36] = "Halt失败。",
            [0x37] = "执行透传命令出错。",
            [0x38] = "防冲突失败。",
        };

        public static SortedList<byte, string> SRI512SRI4KErrorSet = new SortedList<byte, string>()
        {
            [0x10] = "无标签。",
            [0x11] = "检测到标签冲突。",
            [0x12] = "选择电子标签失败。",
            [0x14] = "读标签数据块失败。",
            [0x18] = "获取标签UID失败。",
            [0x19] = "执行PCall16命令后，标签的Chip_ID低4位值不为0。",
        };
    }

    /// <summary>
    /// 返回消息的枚举，其中：
    /// HF_ 前缀为处理帧时遇到的错误
    /// CP_ 前缀为与读写器进行通信时遇到的错误，收到此类错误表示未收到读写器返回的信息
    /// </summary>
    public enum ReturnMessage : byte
    {
        Success = 0x00,
        HF_FrameLenError = 0x10,
        HF_ReaderAddrError = 0x11,
        HF_CRCError = 0x12,
        HF_StatusError = 0x13,
        CP_OpenPortFailed = 0x20,
        CP_PortHasClosed = 0x21,
        CP_TimeOutError = 0x22,
        CP_ReadFaild = 0x23,
        CP_WriteFaild = 0x24
    }
}
