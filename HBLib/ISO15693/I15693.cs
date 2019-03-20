using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.ISO15693
{
    public class I15693 : FrameHelper
    {
        private ComPort com;

        public I15693(byte com_adr, ComPort com) : base(com_adr)
        {
            this.com = com;
        }

        /// <summary>
        /// 符合协议的所有电子标签都能响应，但只是返回一张电子标签的UID，并让这张电子标签进入Quiet状态
        /// </summary>
        /// <returns></returns>
        public async Task<InventoryInfo> InventoryWithoutAFIAsync()
        {
            byte[] frame = CreateInventoryWithoutAFIFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InventoryInfo ii = new InventoryInfo();
                ii.SendByte = frame;
                ii.ReturnValue = cri.ReturnValue;
                ii.ExceptionMessage = cri.ExceptionMessage;
                return ii;
            }

            InventoryInfo info = HandleInventoryFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 带AFI的Inventory。
        /// AFI相符的电子标签才能响应，但只是返回一张电子标签的UID，并让这张电子标签进入Quiet状态
        /// </summary>
        /// <param name="afi">类型识别码</param>
        /// <returns></returns>
        public async Task<InventoryInfo> InventoryWithAFIAsync(byte afi)
        {
            byte[] frame = CreateInventoryWithAFIFrame(afi);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InventoryInfo ii = new InventoryInfo();
                ii.SendByte = frame;
                ii.ReturnValue = cri.ReturnValue;
                ii.ExceptionMessage = cri.ExceptionMessage;
                return ii;
            }

            InventoryInfo info = HandleInventoryFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
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
        public async Task<InventoryScanInfo> InventoryScanWithoutAFIAsync(InventoryScanWithoutAFIMode mode)
        {
            byte[] frame = CreateInventoryScanWithoutAFIFrame(mode);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InventoryScanInfo isi = new InventoryScanInfo();
                isi.SendByte = frame;
                isi.ReturnValue = cri.ReturnValue;
                isi.ExceptionMessage = cri.ExceptionMessage;
                return isi;
            }

            InventoryScanInfo info = HandleInventoryScanFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
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
        /// <param name="afi">类型识别码</param>
        /// <returns></returns>
        public async Task<InventoryScanInfo> InventoryScanWithAFIAsync(InventoryScanWithAFIMode mode, byte afi)
        {
            byte[] frame = CreateInventoryScanWithAFIFrame(mode, afi);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InventoryScanInfo isi = new InventoryScanInfo();
                isi.SendByte = frame;
                isi.ReturnValue = cri.ReturnValue;
                isi.ExceptionMessage = cri.ExceptionMessage;
                return isi;
            }

            InventoryScanInfo info = HandleInventoryScanFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机发送该命令设置指定的电子标签进入“Quiet状态”。在此状态下inventory命令将对这张电子标签无效。
        /// 该命令只能在address mode模式下运行：
        /// 有三种途径可以令标签退出“Quiet状态”：
        /// 1. 电子标签离开读写器的感应场有效范围后重新进入；
        /// 2. 电子标签接收到针对它的“Select”命令，进入selected状态；
        /// 3. 电子标签接收到“Reset to ready”命令，进入ready状态。
        /// </summary>
        /// <param name="uid">指定标签的 UID，长度为 8 字节</param>
        /// <returns></returns>
        public async Task<InfoBase> StayQuietAsync(byte[] uid)
        {
            byte[] frame = CreateStayQuietFrame(uid);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }

            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机发送该命令读取电子标签中指定块的数据（4或8个字节）和安全状态信息。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="blockLen">卡的数据块所占空间的字节数，4或8</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <param name="blockNum">绝对块号</param>
        /// <returns></returns>
        public async Task<ReadSingleBlockInfo> ReadSingleBlockAsync(I15693BlockLen blockLen, byte[] uid, byte blockNum)
        {
            byte[] frame = CreateReadSingleBlockFrame(blockLen, uid, blockNum);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ReadSingleBlockInfo ib = new ReadSingleBlockInfo();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            ReadSingleBlockInfo info = HandleReadSingleBlockFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机发送该命令读取电子标签中指定块的数据（4或8个字节）和安全状态信息。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="blockLen">卡的数据块所占空间的字节数，4或8</param>
        /// <param name="blockNum">绝对块号</param>
        /// <returns></returns>
        public async Task<ReadSingleBlockInfo> ReadSingleBlockAsync(I15693BlockLen blockLen, byte blockNum)
        {
            byte[] frame = CreateReadSingleBlockFrame(blockLen, blockNum);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ReadSingleBlockInfo ib = new ReadSingleBlockInfo();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            ReadSingleBlockInfo info = HandleReadSingleBlockFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
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
        public async Task<InfoBase> WriteSingleBlockAsync(I15693CardType type, I15693BlockLen blockLen,
            byte[] uid, byte blockNum, byte[] data)
        {
            byte[] frame = CreateWriteSingleBlockFrame(type, blockLen, uid, blockNum, data);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
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
        public async Task<InfoBase> WriteSingleBlockAsync(I15693CardType type, I15693BlockLen blockLen,
            byte blockNum, byte[] data)
        {
            byte[] frame = CreateWriteSingleBlockFrame(type, blockLen, blockNum, data);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
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
        public async Task<InfoBase> LockBlockAsync(I15693CardType type, byte[] uid, byte blockNum)
        {
            byte[] frame = CreateLockBlockFrame(type, uid, blockNum);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令以锁定电子标签中指定的块。该块的内容将不能再被修改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="blockNum">绝对块号</param>
        /// <returns></returns>
        public async Task<InfoBase> LockBlockAsync(I15693CardType type, byte blockNum)
        {
            byte[] frame = CreateLockBlockFrame(type, blockNum);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
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
        public async Task<ReadMultipleBlockInfo> ReadMultipleBlockAsync(I15693BlockLen blockLen,
            byte[] uid, byte start, byte count)
        {
            byte[] frame = CreateReadMultipleBlockFrame(blockLen, uid, start, count);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ReadMultipleBlockInfo ib = new ReadMultipleBlockInfo();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            ReadMultipleBlockInfo info = HandleReadMultipleBlockFrame(blockLen, cri.RecvByte);
            info.SendByte = frame;
            return info;
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
        public async Task<ReadMultipleBlockInfo> ReadMultipleBlockAsync(I15693BlockLen blockLen,
            byte start, byte count)
        {
            byte[] frame = CreateReadMultipleBlockFrame(blockLen, start, count);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ReadMultipleBlockInfo ib = new ReadMultipleBlockInfo();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            ReadMultipleBlockInfo info = HandleReadMultipleBlockFrame(blockLen, cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令设置指定电子标签将进入“Seleted状态”。所有以 selected mode模
        /// 式运行的命令都将是针对这张处于selected状态的电子标签来执行。感应场内一次只能
        /// 有一张标签处于selected状态，当有新的标签进入这个状态后，原来的电子标签将恢复
        /// 到Ready状态。
        /// </summary>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <returns></returns>
        public async Task<InfoBase> SelectAsync(byte[] uid)
        {
            byte[] frame = CreateSelectFrame(uid);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令可以将处于Quiet状态的电子标签设置为返回“Ready状态”。
        /// 该命令可以在address mode和non-address mode下运行。
        /// </summary>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <returns></returns>
        public async Task<InfoBase> ResetToReadyAsync(byte[] uid)
        {
            byte[] frame = CreateResetToReadyFrame(uid);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令可以将处于Quiet状态的电子标签设置为返回“Ready状态”。
        /// 该命令可以在address mode和non-address mode下运行。
        /// </summary>
        /// <returns></returns>
        public async Task<InfoBase> ResetToReadyAsync()
        {
            byte[] frame = CreateResetToReadyFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令写入一个字节的数据到电子标签的应用类型识别码（AFI=Application Family Identifier）。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <param name="afi">类型识别码（AFI=Application Family Identifier），长度为 1 个字节</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteAFIAsync(I15693CardType type, byte[] uid, byte afi)
        {
            byte[] frame = CreateWriteAFIFrame(type, uid, afi);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令写入一个字节的数据到电子标签的应用类型识别码（AFI=Application Family Identifier）。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="afi">类型识别码（AFI=Application Family Identifier），长度为 1 个字节</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteAFIAsync(I15693CardType type, byte afi)
        {
            byte[] frame = CreateWriteAFIFrame(type, afi);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令锁定电子标签的应用类型识别码（AFI=application family identifier），AFI一旦被锁定将不能再更改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类，具体情况可参看附录1。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <returns></returns>
        public async Task<InfoBase> LockAFIAsync(I15693CardType type, byte[] uid)
        {
            byte[] frame = CreateLockAFIFrame(type, uid);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令锁定电子标签的应用类型识别码（AFI=application family identifier），AFI一旦被锁定将不能再更改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类，具体情况可参看附录1。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <returns></returns>
        public async Task<InfoBase> LockAFIAsync(I15693CardType type)
        {
            byte[] frame = CreateLockAFIFrame(type);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
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
        public async Task<InfoBase> WriteDSFIDAsync(I15693CardType type, byte[] uid, byte dsfid)
        {
            byte[] frame = CreateWriteAFIFrame(type, uid, dsfid);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令将一个字节的数据写入标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier）。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="dsfid">数据写入标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier）</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteDSFIDAsync(I15693CardType type, byte dsfid)
        {
            byte[] frame = CreateWriteAFIFrame(type, dsfid);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令锁定电子标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier），DSFID一旦被锁定将不能再更改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <param name="uid">电子标签 UID，长度为 8 个字节</param>
        /// <returns></returns>
        public async Task<InfoBase> LockDSFIDAsync(I15693CardType type, byte[] uid)
        {
            byte[] frame = CreateLockDSFIDFrame(type, uid);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令锁定电子标签的数据保存格式识别码（DSFID=Data Storage Format IDentifier），DSFID一旦被锁定将不能再更改。
        /// 此命令为写类型命令，不同厂商的电子标签的响应机制会有所不同，它们可分为A类和B类两大类。
        /// 该命令可以在address mode和select mode下运行。
        /// </summary>
        /// <param name="type">卡类型，A类或B类</param>
        /// <returns></returns>
        public async Task<InfoBase> LockDSFIDAsync(I15693CardType type)
        {
            byte[] frame = CreateLockDSFIDFrame(type);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令以获得电子标签的详细信息，这其中包括Information Flag，UID，DSFID，AFI，Memory，IC reference的信息。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<GetSystemInformationInfo> GetSystemInformationAsync(byte[] uid)
        {
            byte[] frame = CreateGetSystemInformationFrame(uid);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                GetSystemInformationInfo gii = new GetSystemInformationInfo();
                gii.SendByte = frame;
                gii.ReturnValue = cri.ReturnValue;
                gii.ExceptionMessage = cri.ExceptionMessage;
                return gii;
            }
            GetSystemInformationInfo info = HandleGetSystemInformationFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机执行该命令以获得电子标签的详细信息，这其中包括Information Flag，UID，DSFID，AFI，Memory，IC reference的信息。
        /// </summary>
        /// <returns></returns>
        public async Task<GetSystemInformationInfo> GetSystemInformationAsync()
        {
            byte[] frame = CreateGetSystemInformationFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                GetSystemInformationInfo gii = new GetSystemInformationInfo();
                gii.SendByte = frame;
                gii.ReturnValue = cri.ReturnValue;
                gii.ExceptionMessage = cri.ExceptionMessage;
                return gii;
            }
            GetSystemInformationInfo info = HandleGetSystemInformationFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }
    }
}
