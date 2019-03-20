using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.ISO14443A
{
    /// <summary>
    /// ISO 14443A：请求—Request使用的参数
    /// </summary>
    public enum RequestMode : byte
    {
        /// <summary>
        /// 空闲卡
        /// </summary>
        IdleCard = 0x00,
        /// <summary>
        /// 所有卡
        /// </summary>
        AllCard = 0x01
    }

    /// <summary>
    /// ISO 14443A：防冲突2—Anticoll2使用的参数
    /// </summary>
    public enum MultiLable : byte
    {
        /// <summary>
        /// 不允许多张电子标签进入感应场
        /// </summary>
        AllowOne = 0x00,
        /// <summary>
        /// 允许多张电子标签进入感应场
        /// </summary>
        AllowMulti = 0x01
    }

    /// <summary>
    /// ISO 14443A：密钥类型
    /// </summary>
    public enum KeyType : byte
    {
        KeyA = 0x00,
        KeyB = 0x01
    }

    /// <summary>
    /// 值操作类型
    /// </summary>
    public enum OperCode : byte
    {
        /// <summary>
        /// 减值
        /// </summary>
        Decrease = 0xC0,
        /// <summary>
        /// 增值
        /// </summary>
        Increase = 0xC1,
        /// <summary>
        /// 备份
        /// </summary>
        Backup = 0xC2
    }

    public struct I14443Cmd
    {
        /// <summary>
        /// 检查有效感应场范围内是否有标签存在
        /// </summary>
        public const byte Request = 0x41;
        /// <summary>
        /// 检查有效感应场范围内是否有标签存在
        /// </summary>
        public const byte Anticoll = 0x42;
        /// <summary>
        /// 可允许或禁止多张电子标签同时进入感应场，
        /// 处理标签冲突，返回一张电子标签序列号。
        /// </summary>
        public const byte Anticoll2 = 0x71;
        /// <summary>
        /// 处理Ultralight标签冲突，返回一张标签序列号。
        /// </summary>
        public const byte ULAnticoll = 0x7A;
        /// <summary>
        /// 选择一张标签，返回标签的容量。
        /// </summary>
        public const byte Select = 0x43;
        /// <summary>
        /// 证实操作
        /// </summary>
        public const byte Authentication = 0x44;
        /// <summary>
        /// 证实操作
        /// </summary>
        public const byte Authentication2 = 0x72;
        /// <summary>
        /// 证实操作
        /// </summary>
        public const byte AuthKey = 0x73;
        /// <summary>
        /// 将标签挂起，设为HALT状态。
        /// </summary>
        public const byte Halt = 0x45;
        /// <summary>
        /// 读出标签中指定数据块的数据内容。
        /// </summary>
        public const byte Read = 0x46;
        /// <summary>
        /// 将数据写入指定的标签数据块。
        /// </summary>
        public const byte Write = 0x47;
        /// <summary>
        /// 将数据写入Ultralight标签中指定的数据块。
        /// </summary>
        public const byte ULWrite = 0x7B;
        /// <summary>
        /// 初始化钱包块（值块）。
        /// </summary>
        public const byte Initvalue = 0x78;
        /// <summary>
        /// 读取钱包块（值块）数据
        /// </summary>
        public const byte Readvalue = 0x79;
        /// <summary>
        /// 增加值块的数值，将结果保存在标签内部寄存器。
        /// </summary>
        public const byte Increment = 0x48;
        /// <summary>
        /// 减少值块的数值，将结果保存在标签内部寄存器。
        /// </summary>
        public const byte Decrement = 0x49;
        /// <summary>
        /// 检查值块的数据结构，将值块的数据存放到标签内部寄存器。
        /// </summary>
        public const byte Restore = 0x4A;
        /// <summary>
        /// 将标签内部寄存器保存的数据传送到指定的值块。
        /// </summary>
        public const byte Transfer = 0x4B;
        /// <summary>
        /// 保存密钥到读写器密钥存储区。
        /// </summary>
        public const byte LoadKey = 0x4C;
        /// <summary>
        /// 对写入标签的数据进行检查
        /// </summary>
        public const byte CheckWrite = 0x53;
        /// <summary>
        /// 读出读写器内部EEPROM数据
        /// </summary>
        public const byte ReadE2 = 0x61;
        /// <summary>
        /// 将数据写入读写器内部EEPROM
        /// </summary>
        public const byte WriteE2 = 0x62;
        /// <summary>
        /// 对指定值块进行值操作。
        /// </summary>
        public const byte Value = 0x70;
    }
}
