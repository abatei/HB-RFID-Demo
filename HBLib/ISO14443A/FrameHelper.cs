using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.ISO14443A
{
    //由于14443操作帧的 State 部分全部为 0x10，所以在此将 State 统一设置为 0x10
    class Frame : FrameBase
    {
        public Frame(byte len) : base(len) { }
        public Frame(byte len, byte com_adr) : base(len, com_adr)
        {
            DataBlock[3] = 0x10;
        }
    }

    public class FrameHelper : HelperBase
    {
        public FrameHelper(byte com_adr) : base(com_adr) { }

        #region 主机向读写器发送指令时，获取所需帧的操作
        /// <summary>
        /// 获取发送 Request 命令所需的帧，检查在有效范围内是否有电子标签的存在，
        /// 在选择一个新的电子标签进行操作前必须执行此命令。
        /// </summary>
        /// <param name="mode">0：请求IDLE标签；1：请求所有标签</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateRequestFrame(RequestMode mode)
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Request;
            db[4] = (byte)mode;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 防冲突操作所需的帧，必须在Request命令后立即执行。
        /// </summary>
        /// <returns>完整报文</returns>
        protected byte[] CreateAnticollFrame()
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Anticoll;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 防冲突操2作所需的帧，必须在Request命令后立即执行。
        /// </summary>
        /// <param name="ml">选择是否允许多标签进入同一感应场。若为1则允许多张电子标签进入；若为0则不允许多张电子标签进入</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateAnticoll2Frame(MultiLable ml)
        {
            Frame frame = new Frame(0x07, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Anticoll2;
            db[4] = (byte)ml; //压入是否允许多标签进入同一感应场
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// UtraLight标签防冲突操作所需的帧。
        /// </summary>
        /// <returns>完整报文</returns>
        protected byte[] CreateULAnticollFrame()
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.ULAnticoll;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 选择指定UID的电子标签所需的帧
        /// </summary>
        /// <param name="UID">电子标签UID</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateSelectFrame(byte[] UID)
        {
            Frame frame = new Frame(0x09, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Select;
            UID.CopyTo(db, 4);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 证实操作所需的帧
        /// </summary>
        /// <param name="keyType">为0利用密钥A进行验证，为1利用密钥B进行验证</param>
        /// <param name="sectorNum">待证实的电子标签的扇区号，必须小于16。</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateAuthenticationFrame(KeyType keyType, byte sectorNum)
        {
            Frame frame = new Frame(0x07, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Authentication;
            db[4] = (byte)keyType;
            db[5] = sectorNum;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 证实操作所需的帧
        /// </summary>
        /// <param name="keyType">为0利用密钥A进行验证，为1利用密钥B进行验证</param>
        /// <param name="sectorNum">待证实的电子标签的扇区号，必须小于16。</param>
        /// <param name="keySectorNum">读写器密钥存储区中的密钥区号，必须小于16。</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateAuthentication2Frame(KeyType keyType,
            byte sectorNum, byte keySectorNum)
        {
            Frame frame = new Frame(0x08, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Authentication2;
            db[4] = (byte)keyType;
            db[5] = sectorNum;
            db[6] = keySectorNum;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 直接密码证实所需的帧
        /// </summary>
        /// <param name="keyType">为0利用密钥A进行验证，为1利用密钥B进行验证</param>
        /// <param name="sectorNum">待证实的电子标签的扇区号，必须小于16。</param>
        /// <param name="key">用于证实的6字节密钥数据，低字节在前</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateAuthKeyFrame(KeyType keyType,
            byte sectorNum, byte[] key)
        {
            Frame frame = new Frame(0x0D, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.AuthKey;
            db[4] = (byte)keyType;
            db[5] = sectorNum;
            key.CopyTo(db, 6);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 将所选择电子标签置为挂起状态。
        /// </summary>
        /// <returns>完整报文</returns>
        protected byte[] CreateHaltFrame()
        {
            Frame frame = new Frame(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Halt;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 读取标签指定块的16个字节
        /// </summary>
        /// <param name="blockNum">待读取的数据块的绝对块号</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateReadFrame(byte blockNum)
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Read;
            db[4] = blockNum;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 将16字节数据写入标签指定的块
        /// </summary>
        /// <param name="blockNum">待写入数据的数据块的绝对块号</param>
        /// <param name="data">要写入的16字节数据，低字节在前。</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateWriteFrame(byte blockNum, byte[] data)
        {
            Frame frame = new Frame(0x16, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Write;
            db[4] = blockNum;
            data.CopyTo(db, 5);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 写入数据到UltraLight标签所需的帧
        /// </summary>
        /// <param name="blockNum">待写入数据的UltraLight页号</param>
        /// <param name="data">4字节要写入的数据，低字节在前</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateULWriteFrame(byte blockNum, byte[] data)
        {
            Frame frame = new Frame(0x0A, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.ULWrite;
            db[4] = blockNum;
            data.CopyTo(db, 5);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 钱包功能初始化所需的帧
        /// </summary>
        /// <param name="blockNum">指定用做钱包功能的数据块的绝对块号</param>
        /// <param name="value">钱包功能的初始化值，低字节在前</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateInitValueFrame(byte blockNum, byte[] value)
        {
            Frame frame = new Frame(0x0A, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Initvalue;
            db[4] = blockNum;
            value.CopyTo(db, 5);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 读取钱包数据块（值块）的值
        /// </summary>
        /// <param name="blockNum">钱包数据块（值块）的绝对块号</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateReadValueFrame(byte blockNum)
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Readvalue;
            db[4] = blockNum;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 增加值块值所需的帧
        /// </summary>
        /// <param name="blockNum">要操作的值块的绝对块号</param>
        /// <param name="value">要增加的值，低字节在前</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateIncrementFrame(byte blockNum, byte[] value)
        {
            Frame frame = new Frame(0x0A, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Increment;
            db[4] = blockNum;
            value.CopyTo(db, 5);
            frame.PushCRC();
            return db;
        }


        /// <summary>
        /// 减少值块值所需的帧
        /// </summary>
        /// <param name="blockNum">要操作的值块的绝对块号</param>
        /// <param name="value">要减少的值，低字节在前</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateDecrementFrame(byte blockNum, byte[] value)
        {
            Frame frame = new Frame(0x0A, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Decrement;
            db[4] = blockNum;
            value.CopyTo(db, 5);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 转存值块所需的帧
        /// </summary>
        /// <param name="blockNum">要操作的值块的绝对块号</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateRestoreFrame(byte blockNum)
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Restore;
            db[4] = blockNum;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 将标签的内部寄存器内容传送给指定的数据块所需的帧
        /// </summary>
        /// <param name="blockNum">指定的传送的目标块的绝对块号</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateTransferFrame(byte blockNum)
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Transfer;
            db[4] = blockNum;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 将一个新的密钥写入读写器密钥存储区中的指定位置储存
        /// </summary>
        /// <param name="keyType">为0利用密钥A进行验证，为1利用密钥B进行验证</param>
        /// <param name="sectorNum">读写器密钥存储区中的指定位置的扇区号，必须小于16</param>
        /// <param name="key">6字节密钥，低字节在前</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateLoadKeyFrame(KeyType keyType,
            byte sectorNum, byte[] key)
        {
            Frame frame = new Frame(0x0D, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.LoadKey;
            db[4] = (byte)keyType;
            db[5] = sectorNum;
            key.CopyTo(db, 6);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 对写入标签的数据进行检查
        /// </summary>
        /// <param name="UID">电子标签的序列号</param>
        /// <param name="keyType">上次写的证实模式</param>
        /// <param name="sectorNum">所要检查的数据块的绝对块号</param>
        /// <param name="data">所要检查的16字节的数据，低字节在前</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateCheckWriteFrame(byte[] UID, KeyType keyType,
            byte sectorNum, byte[] data)
        {
            Frame frame = new Frame(0x1B, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.CheckWrite;
            UID.CopyTo(db, 4);
            db[8] = (byte)keyType;
            db[9] = sectorNum;
            data.CopyTo(db, 10);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 将指定地址开始的读写器内部EEPROM的数据读出所需的帧
        /// </summary>
        /// <param name="start">读操作的起始地址，必须小于0x80</param>
        /// <param name="len">被读的数据长度（小于20个字节</param>
        /// <returns>完整报文</returns>
        protected byte[] CreateReadE2Frame(byte start, byte len)
        {
            Frame frame = new Frame(0x07, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.ReadE2;
            db[4] = start;
            db[5] = len;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 将数据写入指定地址开始的读写器内部EEPROM。
        /// 地址0x00～0x0F为只读区域，不能写入，0x10～0x2F为初始化数据区，
        /// 不要改写，0x80-0x1FF 为读写器密钥存储区，不能用该命令写入。
        /// </summary>
        /// <param name="start">写操作的起始地址，范围0x30～0x7E</param>
        /// <param name="len">写入的数据长度，必须小于20个字节</param>
        /// <param name="data">要写入的数据，低字节在前</param>
        /// <returns></returns>
        protected byte[] CreateWriteE2Frame(byte start, byte len, byte[] data)
        {
            Frame frame = new Frame((byte)(0x07 + len), Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.WriteE2;
            db[4] = start;
            db[5] = len;
            data.CopyTo(db, 6);
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 对标签内的某一值块进行加、减或备份，支持自动传送，
        /// 传送的目的块与该值块必须在同一个扇区。
        /// </summary>
        /// <param name="oper">为0xC0进行减操作，为0xC1进行加操作，为0xC2进行备份操作</param>
        /// <param name="sourceBlockNum">执行值操作的源值块的绝对块号，取值范围0-63；</param>
        /// <param name="value">当进行加、减操作时，为加数或减数；当进行恢复操作时该值为空值</param>
        /// <param name="destBlockNum">执行操作后保存操作结果的目的值块的绝对块号，取值范围0-63</param>
        /// <returns></returns>
        protected byte[] CreateValueFrame(OperCode oper, byte sourceBlockNum,
            byte[] value, byte destBlockNum)
        {
            Frame frame = new Frame(0x0C, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I14443Cmd.Value;
            db[4] = (byte)oper;
            db[5] = sourceBlockNum;
            value.CopyTo(db, 6);
            db[10] = destBlockNum;
            frame.PushCRC();
            return db;
        }
        #endregion

        #region 主机收到读写器发来的帧时，处理这些帧
        protected RequestInfo HandleRequestFrame(byte[] frame)
        {
            RequestInfo info = new RequestInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }

            Array.Copy(frame, 3, info.CardType, 0, 2); //拷贝版本号
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected AnticollInfo HandleAnticollFrame(byte[] frame)
        {
            AnticollInfo info = new AnticollInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            Array.Copy(frame, 3, info.UID, 0, 4); //拷贝返回的 UID
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected ULAnticollInfo HandleULAnticollFrame(byte[] frame)
        {
            ULAnticollInfo info = new ULAnticollInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            Array.Copy(frame, 3, info.UL_UID, 0, 7); //拷贝返回的 UL_UID
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected SelectInfo HandleSelectFrame(byte[] frame)
        {
            SelectInfo info = new SelectInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            info.Size = frame[3];
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected ReadInfo HandleReadFrame(byte[] frame)
        {
            ReadInfo info = new ReadInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            Array.Copy(frame, 3, info.BlockData, 0, 16); //拷贝返回的块中的数据
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected ReadValueInfo HandleReadValueFrame(byte[] frame)
        {
            ReadValueInfo info = new ReadValueInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            Array.Copy(frame, 3, info.ValueData, 0, 4); //拷贝返回的块中的数据
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected ReadE2Info HandleReadE2Frame(byte[] frame)
        {
            ReadE2Info info = new ReadE2Info(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            if (frame[0] > 4)
            {
                Array.Copy(frame, 3, info.E2Data, 0, info.E2Data.Length); //拷贝返回的 EEPROM 中的数据
            }
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }
        #endregion
    }
}
