using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib.LJYZN105Reader
{
    public class FrameHelper : HelperBase
    {
        public FrameHelper(byte com_adr) : base(com_adr) { }

        #region 主机向读写器发送指令时，获取所需帧的操作
        protected byte[] CreateGetInformationFrame()
        {
            FrameBase frame = new FrameBase(0x04, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = ReaderCmd.GetInformation;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetFrequencyFrame(byte minFre, byte maxFre)
        {
            FrameBase frame = new FrameBase(0x06, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = ReaderCmd.SetFrequency;
            db[3] = maxFre;
            db[4] = minFre;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetAddrFrame(byte address)
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = ReaderCmd.SetAddr;
            db[3] = address;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetInventoryTimeFrame(byte scanTime)
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = ReaderCmd.SetInventoryTime;
            db[3] = scanTime;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetBaudRateFrame(byte baudRate)
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = ReaderCmd.SetBaudRate;
            db[3] = baudRate;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetPowerDbmFrame(byte pwr)
        {
            FrameBase frame = new FrameBase(0x05, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = ReaderCmd.SetPowerDbm;
            db[3] = pwr;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateBuzzerAndLEDControlFrame(byte activeT, byte silentT, byte times)
        {
            FrameBase frame = new FrameBase(0x07, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = ReaderCmd.BuzzerAndLEDControl;
            db[3] = activeT;
            db[4] = silentT;
            db[5] = times;
            frame.PushCRC();
            return db;
        }
        #endregion

        #region 主机收到读写器发来的帧时，处理这些帧
        protected GetInformationInfo HandleGetInformationFrame(byte[] frame)
        {
            GetInformationInfo info = new GetInformationInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }

            Array.Copy(frame, 4, info.Version, 0, 2); //拷贝版本号
            info.Type = frame[6];
            info.TrType = frame[7];
            info.Dmaxfre = frame[8];
            info.Dminfre = frame[9];
            info.Power = frame[10];
            info.Scntm = frame[11];
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        #endregion
    }
}
