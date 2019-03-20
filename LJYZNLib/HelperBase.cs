using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib
{
    public class HelperBase
    {
        public byte Com_adr { get; set; } = 0x00; //读写器地址

        public HelperBase() { }
        public HelperBase(byte com_adr)
        {
            this.Com_adr = com_adr;
        }

        protected InfoBase HandleBaseFrame(byte[] frame)
        {
            InfoBase info = new InfoBase(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        /// <summary>
        /// 检查帧公共部分是否合法
        /// </summary>
        /// <param name="frame">响应的帧</param>
        /// <returns>HandleFrameReturn 枚举类型</returns>
        protected ReturnMessage CheckFrame(InfoBase info)
        {
            byte[] frame = info.RecvByte;
            //如果帧长度不对，则检查不通过
            if (frame[0] != frame.Length - 1)
            {
                return ReturnMessage.HF_FrameLenError;
            }
            //检查读写器地址
            if (frame[1] != Com_adr && frame[1] != 0xFF)
            {
                return ReturnMessage.HF_ReaderAddrError;
            }
            //CRC校验
            UInt16 crc = FrameBase.GetCRC(frame);
            UInt16 a = BitConverter.ToUInt16(frame, frame.Length - 2);
            UInt16 b = FrameBase.Reverse(crc);
            if (BitConverter.ToUInt16(frame, frame.Length - 2) != crc)
            {
                return ReturnMessage.HF_CRCError;
            }
            //压入结果状态码
            info.SetStatus();
            if (info.Status != 0x00)
            {
                return ReturnMessage.HF_StatusError;
            }
            return ReturnMessage.Success;
        }
    }
}
