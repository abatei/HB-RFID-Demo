using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.HR8002Reader
{
    //由于读写器自定义命令帧的 Cmd 部分全部为 0，所以在此将 Cmd 统一设置为 0
    class Frame : FrameBase
    {
        public Frame(byte len) : base(len) { }
        public Frame(byte len, byte com_adr) : base(len, com_adr)
        {
            DataBlock[2] = 0x00;
        }
    }

    public class FrameHelper : HelperBase
    {
        public FrameHelper() { }
        public FrameHelper(byte com_adr) : base(com_adr) { }

        #region 主机向读写器发送指令时，获取所需帧的操作
        /// <summary>
        /// 获得读写器的信息所需的帧
        /// </summary>
        /// <returns>返回完整报文</returns>
        protected byte[] CreateGetReaderInformationFrame(byte? addr)
        {
            Frame frame = new Frame(0x05, addr ?? Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.GetReaderInformation;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 关闭感应场所需的帧
        /// </summary>
        /// <returns>返回完整报文</returns>
        protected byte[] CreateCloseRFFrame()
        {
            Frame frame = new Frame(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.CloseRF;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 打开感应场所需的帧
        /// </summary>
        /// <returns>返回完整报文</returns>
        protected byte[] CreateOpenRFFrame()
        {
            Frame frame = new Frame(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.OpenRF;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 更改读写器地址所需的帧
        /// </summary>
        /// <param name="addr">读写器的新地址</param>
        /// <returns>返回完整报文</returns>
        protected byte[] CreateWriteComAdrFrame(byte addr)
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.WriteComAdr;
            db[4] = addr; //设置读写器的新地址
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 设置读写器询查命令最大响应时间所需的帧
        /// </summary>
        /// <param name="inventoryScanTime">
        /// 范围是0x03~0xff，表示（3*100ms~255*100ms）。
        /// </param>
        /// <returns>返回完整报文</returns>
        protected byte[] CreateWriteInventoryScanTimeFrame(byte inventoryScanTime)
        {
            Frame frame = new Frame(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.WriteInventoryScanTime;
            db[4] = inventoryScanTime; //压入最大响应时间
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 设置读写器为ISO14443A模式所需的帧
        /// </summary>
        /// <returns>返回完整报文</returns>
        protected byte[] CreateChangeToISO14443AFrame()
        {
            Frame frame = new Frame(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.ChangToISO14443A;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 设置读写器为ISO14443B模式所需的帧
        /// </summary>
        /// <returns>返回完整报文</returns>
        protected byte[] CreateChangeToISO14443BFrame()
        {
            Frame frame = new Frame(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.ChangToISO14443B;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 设置读写器为ISO15693A模式所需的帧
        /// </summary>
        /// <returns>返回完整报文</returns>
        protected byte[] CreateChangeToISO15693Frame()
        {
            Frame frame = new Frame(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.ChangToISO15693;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 控制读写器LED的状态所需的帧
        /// </summary>
        /// <param name="duration">持续时间 *50ms</param>
        /// <param name="interval">间隔时间 *50ms</param>
        /// <param name="repeat">重复次数</param>
        /// <returns></returns>
        protected byte[] CreateSetLEDFrame(byte duration, byte interval, byte repeat)
        {
            Frame frame = new Frame(0x08, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.SetLED;
            db[4] = duration;
            db[5] = interval;
            db[6] = repeat;
            frame.PushCRC();
            return db;
        }

        /// <summary>
        /// 控制蜂鸣信号所需的帧
        /// </summary>
        /// <param name="duration">持续时间 *50ms</param>
        /// <param name="interval">间隔时间 *50ms</param>
        /// <param name="repeat">重复次数</param>
        /// <returns></returns>
        protected byte[] CreateBeepFrame(byte duration, byte interval, byte repeat)
        {
            Frame frame = new Frame(0x08, Com_adr);
            byte[] db = frame.DataBlock;
            db[3] = ReaderState.Beep;
            db[4] = duration;
            db[5] = interval;
            db[6] = repeat;
            frame.PushCRC();
            return db;
        }
        #endregion

        #region 主机收到读写器发来的帧时，处理这些帧
        protected GetReaderInformationInfo HandleGetReaderInformationFrame(byte[] frame)
        {
            GetReaderInformationInfo info = new GetReaderInformationInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }

            Array.Copy(frame, 3, info.Version, 0, 2); //拷贝版本号
            info.ReaderType = frame[7]; //拷贝读卡器类型
            Array.Copy(frame, 8, info.ProtocolType, 0, 2); //拷贝协议类型
            info.InventoryScanTime = frame[10];
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }
        #endregion
    }
}
