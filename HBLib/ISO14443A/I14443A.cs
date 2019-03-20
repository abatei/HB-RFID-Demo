using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HBLib.HR8002Reader;

namespace HBLib.ISO14443A
{
    public class I14443A : FrameHelper
    {
        private ComPort com;

        public I14443A(byte com_adr, ComPort com) : base(com_adr)
        {
            this.com = com;
        }

        /// <summary>
        /// 发送 Request 命令，检查在有效范围内是否有电子标签的存在，
        /// 在选择一个新的电子标签进行操作前必须执行此命令。
        /// </summary>
        /// <param name="mode">
        /// Mode=RequestMode.IdleCard 请求天线范围内处于IDLE 状态的电子标签（处于HALT状态的除外）
        /// Mode=RequestMode.AllCard 请求天线范围内所有的电子标签。
        /// </param>
        /// <returns></returns>
        public async Task<RequestInfo> RequestAsync(RequestMode mode)
        {
            byte[] frame = CreateRequestFrame(mode);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                RequestInfo ri = new RequestInfo();
                ri.SendByte = frame;
                ri.ReturnValue = cri.ReturnValue;
                ri.ExceptionMessage = cri.ExceptionMessage;
                return ri;
            }

            RequestInfo info = HandleRequestFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 防冲突操作，必须在Request命令后立即执行。
        /// 如果事先知道所需要操作的电子标签的序列号，可以越过此调用，
        /// 在Request命令后直接调用Select 即可。
        /// </summary>
        /// <returns></returns>
        public async Task<AnticollInfo> AnticollAsync()
        {
            byte[] frame = CreateAnticollFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                AnticollInfo ai = new AnticollInfo();
                ai.SendByte = frame;
                ai.ReturnValue = cri.ReturnValue;
                ai.ExceptionMessage = cri.ExceptionMessage;
                return ai;
            }

            AnticollInfo info = HandleAnticollFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 防冲突操作，必须在Request命令后立即执行。如果事先知道所需要
        /// 操作的电子标签的序列号，可以越过此调用，在Request命令后直接
        /// 调用Select 即可。
        /// </summary>
        /// <param name="ml">
        /// ml=MultiLable.AllowOne 不允许多张电子标签进入感应场
        /// m1=MultiLable.AllowMulti 允许多张电子标签进入感应场
        /// </param>
        /// <returns></returns>
        public async Task<AnticollInfo> Anticoll2Async(MultiLable ml)
        {
            byte[] frame = CreateAnticoll2Frame(ml);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                AnticollInfo ai = new AnticollInfo();
                ai.SendByte = frame;
                ai.ReturnValue = cri.ReturnValue;
                ai.ExceptionMessage = cri.ExceptionMessage;
                return ai;
            }

            AnticollInfo info = HandleAnticollFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// UtraLight标签防冲突操作，返回一张标签的UID。
        /// </summary>
        /// <returns></returns>
        public async Task<ULAnticollInfo> ULAnticollAsync()
        {
            byte[] frame = CreateULAnticollFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ULAnticollInfo uai = new ULAnticollInfo();
                uai.SendByte = frame;
                uai.ReturnValue = cri.ReturnValue;
                uai.ExceptionMessage = cri.ExceptionMessage;
                return uai;
            }

            ULAnticollInfo info = HandleULAnticollFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 选择指定UID的电子标签，返回该标签的容量标志。
        /// </summary>
        /// <param name="UID">指定的UID</param>
        /// <returns></returns>
        public async Task<SelectInfo> SelectAsync(byte[] UID)
        {
            byte[] frame = CreateSelectFrame(UID);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                SelectInfo si = new SelectInfo();
                si.SendByte = frame;
                si.ReturnValue = cri.ReturnValue;
                si.ExceptionMessage = cri.ExceptionMessage;
                return si;
            }

            SelectInfo info = HandleSelectFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 对标签数据区进行读写加减等操作前，必须对电子标签进行证实操作。
        /// 该操作将进行标签中指定扇区的密钥与读写器密钥存储区中对应位置
        /// 储存的密钥的匹配检查，若匹配，则证实成功
        /// </summary>
        /// <param name="keyType">ISO 14443A：密钥类型，KeyA 或 KeyB</param>
        /// <param name="sectorNum">待证实的电子标签的扇区号，必须小于16。</param>
        /// <returns></returns>
        public async Task<InfoBase> AuthenticationAsync(KeyType keyType, byte sectorNum)
        {
            byte[] frame = CreateAuthenticationFrame(keyType, sectorNum);
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
        /// 对标签数据区进行读写加减等操作前，必须对电子标签进行证实操作。
        /// 该操作将进行标签中指定扇区的密钥与读写器密钥存储区中指定位置
        /// 储存的密钥的交叉匹配检查，若匹配，则证实成功。
        /// </summary>
        /// <param name="keyType">ISO 14443A：密钥类型，KeyA 或 KeyB</param>
        /// <param name="sectorNum">待证实的电子标签的扇区号，必须小于16。</param>
        /// <param name="keySectorNum">读写器密钥存储区中的密钥区号，必须小于16。</param>
        /// <returns></returns>
        public async Task<InfoBase> Authentication2Async(KeyType keyType,
            byte sectorNum, byte keySectorNum)
        {
            byte[] frame = CreateAuthentication2Frame(keyType, sectorNum, keySectorNum);
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
        /// 直接使用密钥进行证实。
        /// 该操作将进行标签中指定扇区的密钥与命令中给出的密钥数据的匹配
        /// 检查，若匹配，则证实成功。
        /// </summary>
        /// <param name="keyType">ISO 14443A：密钥类型，KeyA 或 KeyB</param>
        /// <param name="sectorNum">待证实的电子标签的扇区号，必须小于16。</param>
        /// <param name="key">用于证实的6字节密钥数据，低字节在前。</param>
        /// <returns></returns>
        public async Task<InfoBase> AuthKeyAsync(KeyType keyType,
            byte sectorNum, byte[] key)
        {
            byte[] frame = CreateAuthKeyFrame(keyType, sectorNum, key);
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
        /// 将所选择电子标签置为挂起状态。
        /// 标签可以以下方式离开挂起状态：
        ///   1．应用ALL模式调用Request；
        ///   2．电子标签离开感应场后再进入或执行关/开射频场命令操作。
        /// </summary>
        /// <returns></returns>
        public async Task<InfoBase> HaltAsync()
        {
            byte[] frame = CreateHaltFrame();
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
        /// 读取标签指定块的16个字节
        /// </summary>
        /// <param name="blockNum">待读取的数据块的绝对块号</param>
        /// <returns></returns>
        public async Task<ReadInfo> ReadAsync(byte blockNum)
        {
            byte[] frame = CreateReadFrame(blockNum);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ReadInfo ri = new ReadInfo();
                ri.SendByte = frame;
                ri.ReturnValue = cri.ReturnValue;
                ri.ExceptionMessage = cri.ExceptionMessage;
                return ri;
            }

            ReadInfo info = HandleReadFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 将16字节数据写入标签指定的块。
        /// </summary>
        /// <param name="blockNum">待写入数据的数据块的绝对块号</param>
        /// <param name="data">要写入的16字节数据，低字节在前</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteAsync(byte blockNum, byte[] data)
        {
            byte[] frame = CreateWriteFrame(blockNum, data);
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
        /// 写入数据到UltraLight标签
        /// </summary>
        /// <param name="blockNum">待写入数据的UltraLight页号</param>
        /// <param name="data">4字节要写入的数据，低字节在前</param>
        /// <returns></returns>
        public async Task<InfoBase> ULWriteAsync(byte blockNum, byte[] data)
        {
            byte[] frame = CreateULWriteFrame(blockNum, data);
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
        /// 将指定用做钱包功能的数据块初始化成钱包数据格式（值块）
        /// </summary>
        /// <param name="blockNum">指定用做钱包功能的数据块的绝对块号</param>
        /// <param name="data">钱包功能的初始化值，低字节在前</param>
        /// <returns></returns>
        public async Task<InfoBase> InitValueAsync(byte blockNum, byte[] data)
        {
            byte[] frame = CreateInitValueFrame(blockNum, data);
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
        /// 读取钱包数据块（值块）的值
        /// </summary>
        /// <param name="blockNum">钱包数据块（值块）的绝对块号</param>
        /// <returns></returns>
        public async Task<ReadValueInfo> ReadValueAsync(byte blockNum)
        {
            byte[] frame = CreateReadValueFrame(blockNum);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ReadValueInfo ib = new ReadValueInfo();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }

            ReadValueInfo info = HandleReadValueFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 读被访问的值块，检查数据的结构，加上传输的值，然后将
        /// 结果存在标签的内部寄存器中。值块有标准的格式
        /// </summary>
        /// <param name="blockNum">要操作的值块的绝对块号</param>
        /// <param name="data">要增加的值，低字节在前</param>
        /// <returns></returns>
        public async Task<InfoBase> IncrementAsync(byte blockNum, byte[] data)
        {
            byte[] frame = CreateIncrementFrame(blockNum, data);
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
        /// 读被访问的值块，检查数据的结构，减去传输的值，然后将结果
        /// 存在标签的内部寄存器中。值块有标准的格式。
        /// </summary>
        /// <param name="blockNum">要操作的值块的绝对块号</param>
        /// <param name="data">要减少的值，低字节在前</param>
        /// <returns></returns>
        public async Task<InfoBase> DecrementAsync(byte blockNum, byte[] data)
        {
            byte[] frame = CreateDecrementFrame(blockNum, data);
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
        /// 读被访问的值块，检查数据的结构，然后将结果存在标签的
        /// 内部寄存器中。值块有标准的格式。
        /// </summary>
        /// <param name="blockNum">要操作的值块的绝对块号</param>
        /// <returns></returns>
        public async Task<InfoBase> RestoreAsync(byte blockNum)
        {
            byte[] frame = CreateRestoreFrame(blockNum);
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
        /// 传输。将标签的内部寄存器内容传送给指定的数据块。此操作前必须
        /// 通过证实，同时，该操作只能紧跟在Increment、 Decrement 、
        /// Restore操作后进行。
        /// </summary>
        /// <param name="blockNum">指定的传输的目标块的绝对块号</param>
        /// <returns></returns>
        public async Task<InfoBase> TransferAsync(byte blockNum)
        {
            byte[] frame = CreateTransferFrame(blockNum);
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
        /// 将一个新的密钥写入读写器密钥存储区中的指定位置储存
        /// </summary>
        /// <param name="keyType">ISO 14443A：密钥类型，KeyA 或 KeyB</param>
        /// <param name="sectorNum">读写器密钥存储区中的指定位置的扇区号，必须小于16</param>
        /// <param name="key">6字节密钥，低字节在前</param>
        /// <returns></returns>
        public async Task<InfoBase> LoadKeyAsync(KeyType keyType,
            byte sectorNum, byte[] key)
        {
            byte[] frame = CreateLoadKeyFrame(keyType,
                sectorNum, key);
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
        /// 对写入标签的数据进行检查。
        /// 先进行Request/select/ Authentication操作，然后进行指定的数据块的
        /// 内容与命令中给出的数据的比较，数据不符返回Error code 0x2C，其它
        /// 情况返回相应的值。此过程中采用的证实过程参考前面的Authentication操作介绍。
        /// </summary>
        /// <param name="UID">电子标签的序列号</param>
        /// <param name="keyType">上次写的证实模式：密钥类型，KeyA 或 KeyB</param>
        /// <param name="sectorNum">所要检查的数据块的绝对块号</param>
        /// <param name="data">所要检查的16字节的数据，低字节在前。</param>
        /// <returns></returns>
        public async Task<InfoBase> CheckWriteAsync(byte[] UID, KeyType keyType,
            byte sectorNum, byte[] data)
        {
            byte[] frame = CreateCheckWriteFrame(UID, keyType,
                sectorNum, data);
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
        /// 将指定地址开始的读写器内部EEPROM的数据读出。该起始地址必须
        /// 小于0x80。从0x80地址开始是读写器密钥存储区，内容不能被读出。
        /// </summary>
        /// <param name="start">读操作的起始地址，必须小于0x80</param>
        /// <param name="len">被读的数据长度（小于20个字节）</param>
        /// <returns></returns>
        public async Task<ReadE2Info> ReadE2Async(byte start, byte len)
        {
            byte[] frame = CreateReadE2Frame(start, len);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ReadE2Info rei = new ReadE2Info();
                rei.SendByte = frame;
                rei.ReturnValue = cri.ReturnValue;
                rei.ExceptionMessage = cri.ExceptionMessage;
                return rei;
            }

            ReadE2Info info = HandleReadE2Frame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 将数据写入指定地址开始的读写器内部EEPROM。
        /// 地址0x00～0x0F为只读区域，不能写入，0x10～0x2F为初始化数据区，
        /// 不要改写，0x80-0x1FF 为读写器密钥存储区，不能用该命令写入。
        /// </summary>
        /// <param name="start">写操作的起始地址，范围0x30～0x7E；</param>
        /// <param name="len">写入的数据长度，必须小于20个字节；</param>
        /// <param name="data">要写入的数据，低字节在前。</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteE2Async(byte start, byte len, byte[] data)
        {
            byte[] frame = CreateWriteE2Frame(start, len, data);
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
        /// 对标签内的某一值块进行加、减或备份，支持自动传送，传送的目的
        /// 块与该值块必须在同一个扇区。
        /// </summary>
        /// <param name="oper">为0xC0进行减操作，为0xC1进行加操作，为0xC2进行备份操作</param>
        /// <param name="sourceBlockNum">执行值操作的源值块的绝对块号，取值范围0-63；</param>
        /// <param name="value">当进行加、减操作时，为加数或减数；当进行恢复操作时该值为空值</param>
        /// <param name="destBlockNum">执行操作后保存操作结果的目的值块的绝对块号，取值范围0-63</param>
        /// <returns></returns>
        public async Task<InfoBase> ValueAsync(OperCode oper, byte sourceBlockNum,
            byte[] value, byte destBlockNum)
        {
            byte[] frame = CreateValueFrame(oper, sourceBlockNum,
                value, destBlockNum);
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
    }
}
