using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.ISO15693
{
    public class FrameHelper : HelperBase
    {
        public FrameHelper(byte com_adr) : base(com_adr) { }

        #region 主机向读写器发送指令时，获取所需帧的操作
        /// <summary>
        /// 不带AFI的Inventory。
        /// 符合协议的所有电子标签都能响应，但只是返回一张电子标签的UID，并让这张电子标签进入Quiet状态
        /// </summary>
        /// <returns></returns>
        protected byte[] CreateInventoryWithoutAFIFrame()
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.Inventory;
            db[3] = 0x00;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 带AFI的Inventory。
        /// AFI相符的电子标签才能响应，但只是返回一张电子标签的UID，并让这张电子标签进入Quiet状态
        /// </summary>
        /// <param name="AFI">类型识别码</param>
        /// <returns></returns>
        protected byte[] CreateInventoryWithAFIFrame(byte AFI)
        {
            FrameBase frame = new FrameBase(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.Inventory;
            db[3] = 0x01;
            db[4] = AFI;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 不带AFI的Inventory-scan。
        /// 新询查命令：在运行询查命令前，读写器会自动运行“Close RF”和“Open RF”。
        /// 这样，处于感应场内的符合协议的所有电子标签均能响应，读写器会把在
        /// InventoryScanTime溢出前得到的UID全部返回。
        /// 继续询查命令：读写器不会对感应场进行操作，只有新进入感应场的电子标签或
        /// 前面的询查命令没有得到UID的电子标签才会响应，读写器会把在
        /// InventoryScanTime溢出前得到的UID返回
        /// </summary>
        /// <param name="mode">0x06：新询查命令，0x02：继续询查命令</param>
        /// <returns></returns>
        protected byte[] CreateInventoryScanWithoutAFIFrame(InventoryScanWithoutAFIMode mode)
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.Inventory;
            db[3] = (byte)mode;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 带AFI的Inventory-scan。
        /// 新询查命令：在运行询查命令前，读写器会自动运行“Close RF”和“Open RF”。
        /// 这样，处于感应场内的AFI相符的电子标签均能响应，读写器会把在
        /// InventoryScanTime溢出前得到的UID全部返回
        /// 继续询查命令：读写器不会对感应场进行操作，只有新进入感应场的AFI相符的电
        /// 子标签或前面的询查命令没有得到UID的AFI相符的电子标签才会响应，并把在
        /// InventoryScanTime溢出前得到的UID返回
        /// </summary>
        /// <param name="mode">0x07：新询查命令，0x03：继续询查命令</param>
        /// <param name="afi">类型识别码<</param>
        /// <returns></returns>
        protected byte[] CreateInventoryScanWithAFIFrame(InventoryScanWithAFIMode mode, byte afi)
        {
            FrameBase frame = new FrameBase(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.Inventory;
            db[3] = (byte)mode;
            db[4] = afi;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机发送该命令设置指定的电子标签进入“Quiet状态”。在此状态下inventory命令将对这张电子标签无效。
        /// 该命令只能在address mode模式下运行：
        /// 有三种途径可以令标签退出“Quiet状态”：
        /// 1. 电子标签离开读写器的感应场有效范围后重新进入；
        /// 2. 电子标签接收到针对它的“Select”命令，进入selected状态；
        /// 3. 电子标签接收到“Reset to ready”命令，进入ready状态。
        /// </summary>
        /// <param name="UID">指定标签的 UID，长度为 8 字节</param>
        /// <returns></returns>
        protected byte[] CreateStayQuietFrame(byte[] UID)
        {
            FrameBase frame = new FrameBase(0x0d, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.StayQuiet;
            db[3] = 0x00;
            UID.CopyTo(db, 4);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机发送该命令读取电子标签中指定块的数据（4或8个字节）和安全状态信息。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="blockLen">卡的数据块所占空间的字节数，4或8</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <param name="blockNum">绝对块号</param>
        /// <returns></returns>
        protected byte[] CreateReadSingleBlockFrame(I15693BlockLen blockLen, byte[] uid, byte blockNum)
        {
            FrameBase frame = new FrameBase(0x0E, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.ReadSingleBlock;
            if (blockLen == I15693BlockLen.Four) //块长度为 4 时
            {
                db[3] = 0x00;
            }
            else //块长度为 8 时
            {
                db[3] = 0x04;
            }
            uid.CopyTo(db, 4);
            db[12] = blockNum;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机发送该命令读取电子标签中指定块的数据（4或8个字节）和安全状态信息。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="blockLen">卡的数据块所占空间的字节数，4或8</param>
        /// <param name="blockNum">绝对块号</param>
        /// <returns></returns>
        protected byte[] CreateReadSingleBlockFrame(I15693BlockLen blockLen, byte blockNum)
        {
            FrameBase frame = new FrameBase(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.ReadSingleBlock;
            if (blockLen == I15693BlockLen.Four) //块长度为 4 时
            {
                db[3] = 0x01;
            }
            else //块长度为 8 时
            {
                db[3] = 0x05;
            }
            db[4] = blockNum;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机发送该命令将给定数据（4或8个字节）写入电子标签的指定数据块中。。
        /// 上位机可指定的块的范围和每个块的大小会因电子标签的生产厂商的不同而有所差异。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="blockLen">卡的数据块所占空间的字节数，4或8</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <param name="blockNum">绝对块号</param>
        /// <param name="data">写入的数据</param>
        /// <returns></returns>
        protected byte[] CreateWriteSingleBlockFrame(I15693CardType type, I15693BlockLen blockLen,
            byte[] uid, byte blockNum, byte[] data)
        {
            int len = (blockLen == I15693BlockLen.Four) ? 0x12 : 0x16;

            FrameBase frame = new FrameBase((byte)len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.WriteSingleBlock;
            //压入 State
            if (blockLen == I15693BlockLen.Four) //块长度为 4 时
            {
                if (type == I15693CardType.TypeA) //为 A 类电子标签时
                {
                    db[3] = (byte)0x00;
                }
                else //为 B 类电子标签时
                {
                    db[3] = (byte)0x08;
                }
            }
            else //块长度为 8 时
            {
                if (type == I15693CardType.TypeA) //为 A 类电子标签时
                {
                    db[3] = (byte)0x04;
                }
                else //为 B 类电子标签时
                {
                    db[3] = (byte)0x0C;
                }
            }
            //压入 UID
            uid.CopyTo(db, 4);
            db[12] = blockNum; //压入块号
            data.CopyTo(db, 13); //压入要写入的数据
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机发送该命令将给定数据（4或8个字节）写入电子标签的指定数据块中。。
        /// 上位机可指定的块的范围和每个块的大小会因电子标签的生产厂商的不同而有所差异。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="blockLen">卡的数据块所占空间的字节数，4或8</param>
        /// <param name="blockNum">绝对块号</param>
        /// <param name="data">写入的数据</param>
        /// <returns></returns>
        protected byte[] CreateWriteSingleBlockFrame(I15693CardType type, I15693BlockLen blockLen,
            byte blockNum, byte[] data)
        {
            int len = (blockLen == I15693BlockLen.Four) ? 0x0A : 0x0E;

            FrameBase frame = new FrameBase((byte)len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.WriteSingleBlock;
            //压入 State
            if (blockLen == I15693BlockLen.Four) //块长度为 4 时
            {
                if (type == I15693CardType.TypeA) //为 A 类电子标签时
                {
                    db[3] = (byte)0x01;
                }
                else //为 B 类电子标签时
                {
                    db[3] = (byte)0x09;
                }
            }
            else //块长度为 8 时
            {
                if (type == I15693CardType.TypeA) //为 A 类电子标签时
                {
                    db[3] = (byte)0x05;
                }
                else //为 B 类电子标签时
                {
                    db[3] = (byte)0x0D;
                }
            }
            //压入 UID
            db[4] = blockNum; //压入块号
            data.CopyTo(db, 5); //压入要写入的数据
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令以锁定电子标签中指定的块。该块的内容将不能再被修改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <param name="blockNum">绝对块号</param>
        /// <returns></returns>
        protected byte[] CreateLockBlockFrame(I15693CardType type, byte[] uid, byte blockNum)
        {
            FrameBase frame = new FrameBase(0x0E, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.LockBlock;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x00;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x08;
            }
            //压入 UID
            uid.CopyTo(db, 4);
            db[12] = blockNum; //压入块号
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令以锁定电子标签中指定的块。该块的内容将不能再被修改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="blockNum">绝对块号</param>
        /// <returns></returns>
        protected byte[] CreateLockBlockFrame(I15693CardType type, byte blockNum)
        {
            FrameBase frame = new FrameBase(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.LockBlock;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x01;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x09;
            }
            //压入 UID
            db[4] = blockNum; //压入块号
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机发送该命令读取电子标签中多个指定块的数据（每个块4或8个字节）和安全状态信息。
        /// 当块的长度为4个字节时一次最多能读12个块，当块的长度为8个字节时一次最多能读6个块。
        /// 上位机可指定的块的范围和每个块的大小因电子标签的生产厂商的不同而有所差异。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="blockLen">卡的数据块所占空间的字节数，4或8</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <param name="start">读取的开始块号</param>
        /// <param name="count">读取的块的个数</param>
        /// <returns></returns>
        protected byte[] CreateReadMultipleBlockFrame(I15693BlockLen blockLen,
            byte[] uid, byte start, byte count)
        {
            FrameBase frame = new FrameBase(0x0F, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.ReadMultipleBlock;
            //压入 State
            if (blockLen == I15693BlockLen.Four) //块的字节数为4时的情况
            {
                db[3] = (byte)0x00;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x04;
            }
            //压入 UID
            uid.CopyTo(db, 4);
            db[12] = start; //压入开始块号
            db[13] = count; //压入读取长度
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机发送该命令读取电子标签中多个指定块的数据（每个块4或8个字节）和安全状态信息。
        /// 当块的长度为4个字节时一次最多能读12个块，当块的长度为8个字节时一次最多能读6个块。
        /// 上位机可指定的块的范围和每个块的大小因电子标签的生产厂商的不同而有所差异。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="blockLen">卡的数据块所占空间的字节数，4或8</param>
        /// <param name="start">读取的开始块号</param>
        /// <param name="count">读取的块的个数</param>
        /// <returns></returns>
        protected byte[] CreateReadMultipleBlockFrame(I15693BlockLen blockLen,
            byte start, byte count)
        {
            FrameBase frame = new FrameBase(0x07, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.ReadMultipleBlock;
            //压入 State
            if (blockLen == I15693BlockLen.Four) //块的字节数为4时的情况
            {
                db[3] = (byte)0x01;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x05;
            }
            db[4] = start; //压入开始块号
            db[5] = count; //压入读取长度
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令设置指定电子标签将进入“Seleted状态”。所有以 selected mode模
        /// 式运行的命令都将是针对这张处于selected状态的电子标签来执行。感应场内一次只能
        /// 有一张标签处于selected状态，当有新的标签进入这个状态后，原来的电子标签将恢复
        /// 到Ready状态。
        /// </summary>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <returns></returns>
        protected byte[] CreateSelectFrame(byte[] uid)
        {
            FrameBase frame = new FrameBase(0x0D, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.Select;
            db[3] = (byte)0x00; //压入 State
            uid.CopyTo(db, 4);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令可以将处于Quiet状态的电子标签设置为返回“Ready状态”。
        /// 该命令可以在address mode和non-address mode下运行。
        /// </summary>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <returns></returns>
        protected byte[] CreateResetToReadyFrame(byte[] uid)
        {
            FrameBase frame = new FrameBase(0x0D, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.ResetToReady;
            db[3] = (byte)0x00; //压入 State
            uid.CopyTo(db, 4);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令可以将处于Quiet状态的电子标签设置为返回“Ready状态”。
        /// 该命令可以在address mode和non-address mode下运行。
        /// </summary>
        /// <returns></returns>
        protected byte[] CreateResetToReadyFrame()
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.ResetToReady;
            db[3] = (byte)0x01; //压入 State
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令写入一个字节的数据到电子标签的应用类型识别码（AFI=Application Family Identifier）。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <param name="afi">类型识别码（AFI=Application Family Identifier），长度为 1 个字节</param>
        /// <returns></returns>
        protected byte[] CreateWriteAFIFrame(I15693CardType type, byte[] uid, byte afi)
        {
            FrameBase frame = new FrameBase(0x0E, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.WriteDSFID;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x00;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x08;
            }
            uid.CopyTo(db, 4);
            db[12] = afi;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令写入一个字节的数据到电子标签的应用类型识别码（AFI=Application Family Identifier）。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="afi">类型识别码（AFI=Application Family Identifier），长度为 1 个字节</param>
        /// <returns></returns>
        protected byte[] CreateWriteAFIFrame(I15693CardType type, byte afi)
        {
            FrameBase frame = new FrameBase(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.WriteAFI;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x01;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x09;
            }
            db[4] = afi;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令锁定电子标签的应用类型识别码（AFI=application family identifier），AFI一旦被锁定将不能再更改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类，具体情况可参看附录1。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <returns></returns>
        protected byte[] CreateLockAFIFrame(I15693CardType type, byte[] uid)
        {
            FrameBase frame = new FrameBase(0x0D, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.LockAFI;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x00;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x08;
            }
            uid.CopyTo(db, 4);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令锁定电子标签的应用类型识别码（AFI=application family identifier），AFI一旦被锁定将不能再更改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类，具体情况可参看附录1。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <returns></returns>
        protected byte[] CreateLockAFIFrame(I15693CardType type)
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.LockAFI;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x01;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x09;
            }
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令将一个字节的数据写入标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier）。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <param name="dsfid">数据写入标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier）</param>
        /// <returns></returns>
        protected byte[] CreateWriteDSFIDFrame(I15693CardType type, byte[] uid, byte dsfid)
        {
            FrameBase frame = new FrameBase(0x0E, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.WriteDSFID;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x00;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x08;
            }
            uid.CopyTo(db, 4);
            db[12] = dsfid;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令将一个字节的数据写入标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier）。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="dsfid">数据写入标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier）</param>
        /// <returns></returns>
        protected byte[] CreateWriteDSFIDFrame(I15693CardType type, byte dsfid)
        {
            FrameBase frame = new FrameBase(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.WriteDSFID;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x01;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x09;
            }
            db[4] = dsfid;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令锁定电子标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier），DSFID一旦被锁定将不能再更改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <returns></returns>
        protected byte[] CreateLockDSFIDFrame(I15693CardType type, byte[] uid)
        {
            FrameBase frame = new FrameBase(0x0D, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.LockDSFID;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x00;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x08;
            }
            uid.CopyTo(db, 4);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令锁定电子标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier），DSFID一旦被锁定将不能再更改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <returns></returns>
        protected byte[] CreateLockDSFIDFrame(I15693CardType type)
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.LockDSFID;
            //压入 State
            if (type == I15693CardType.TypeA) //为 A 类电子标签时
            {
                db[3] = (byte)0x01;
            }
            else //为 B 类电子标签时
            {
                db[3] = (byte)0x09;
            }
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令以获得电子标签的详细信息，这其中包括Information Flag，UID，DSFID，AFI，Memory，IC reference的信息。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        protected byte[] CreateGetSystemInformationFrame(byte[] uid)
        {
            FrameBase frame = new FrameBase(0x0D, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.GetSystemInformation;
            //压入 State
            db[3] = (byte)0x00;
            uid.CopyTo(db, 4);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 上位机执行该命令以获得电子标签的详细信息，这其中包括Information Flag，UID，DSFID，AFI，Memory，IC reference的信息。
        /// </summary>
        /// <returns></returns>
        protected byte[] CreateGetSystemInformationFrame()
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I15693Cmd.GetSystemInformation;
            //压入 State
            db[3] = (byte)0x01;
            frame.PushCRC();
            return db;
        }
        #endregion

        #region 主机收到读写器发来的帧时，处理这些帧
        protected InventoryInfo HandleInventoryFrame(byte[] frame)
        {
            InventoryInfo info = new InventoryInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }

            info.DSFID = frame[3]; //拷贝 DSFID
            Array.Copy(frame, 4, info.UID, 0, 8); //拷贝UID
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected InventoryScanInfo HandleInventoryScanFrame(byte[] frame)
        {
            InventoryScanInfo info = new InventoryScanInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            int len = frame.Length - 5;
            byte[] cardData = new byte[len];
            Array.Copy(frame, 3, cardData, 0, len);
            List<InventoryScanInfo.CardInfo> cardSet = info.CardSet;
            int index = 0;
            while (index < len)
            {
                InventoryScanInfo.CardInfo cardInfo = new InventoryScanInfo.CardInfo();
                cardInfo.DSFID = cardData[index++]; //拷贝 DSFID
                Array.Copy(cardData, index, cardInfo.UID, 0, 8);
                index += 8;
                cardSet.Add(cardInfo);
            }
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected ReadSingleBlockInfo HandleReadSingleBlockFrame(byte[] frame)
        {
            ReadSingleBlockInfo info;
            if (frame.Length == 0x0A)
            {
                info = new ReadSingleBlockInfo(I15693BlockLen.Four, frame);
            }
            else
            {
                info = new ReadSingleBlockInfo(I15693BlockLen.Eight, frame);
            }
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }

            info.BlockSecurityStatus = frame[3]; //块安全状态编码
            if (frame.Length == 0x0A)
            {
                Array.Copy(frame, 4, info.BlockData, 0, 4); //拷贝数据
            }
            else
            {
                Array.Copy(frame, 4, info.BlockData, 0, 8);
            }
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected ReadMultipleBlockInfo HandleReadMultipleBlockFrame(I15693BlockLen blockLen, byte[] frame)
        {
            int dataLen = frame.Length - 5;
            ReadMultipleBlockInfo info = new ReadMultipleBlockInfo(blockLen, frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }

            Array.Copy(frame, 3, info.Data, 0, dataLen);
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected GetSystemInformationInfo HandleGetSystemInformationFrame(byte[] frame)
        {
            GetSystemInformationInfo info = new GetSystemInformationInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            info.InformationFlag = frame[3];
            Array.Copy(frame, 4, info.UID, 0, 8); //拷贝UID
            info.DSFID = frame[12]; //拷贝 DSFID
            info.AFI = frame[13];
            Array.Copy(frame, 14, info.MemorySize, 0, 2);
            info.ICReference = frame[16];
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }
        #endregion
    }
}
