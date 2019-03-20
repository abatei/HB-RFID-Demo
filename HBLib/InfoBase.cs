using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib
{
    /// <summary>
    /// 响应块返回信息基类
    /// </summary>
    public class InfoBase
    {
        public byte Status { get; set; } //结果状态值
        public byte ErrorCode { get; set; } //错误编码
        public byte[] SendByte { get; set; } //发送的字节
        public byte[] RecvByte { get; set; } //接收的字节
        public string ExceptionMessage { get; set; } //异常消息
        public ReturnMessage ReturnValue { get; set; }

        public InfoBase() { }
        public InfoBase(byte[] frame)
        {
            RecvByte = frame;
        }

        public void SetStatus()
        {
            Status = RecvByte[2];
            if (Status == 0x0F || Status == 0x10 || Status == 0x1B || Status == 0x11)
            {
                ErrorCode = RecvByte[3];
            }
        }

        public string GetStatusStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CodeInterpret.StatusSet[Status]);
            switch (Status)
            {
                case 0x0F: //ISO15693协议操作出错
                    sb.Append("：");
                    sb.Append(CodeInterpret.I15693ErrorSet[ErrorCode]);
                    break;
                case 0x10: //ISO14443A协议操作出错
                    sb.Append("：");
                    sb.Append(CodeInterpret.I14443AErrorSet[ErrorCode]);
                    break;
                case 0x1B:
                    sb.Append("：");
                    sb.Append(CodeInterpret.I14443BErrorSet[ErrorCode]);
                    break;
                case 0x11:
                    sb.Append("：");
                    sb.Append(CodeInterpret.SRI512SRI4KErrorSet[ErrorCode]);
                    break;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取发送字节数组的字符串表示
        /// </summary>
        /// <returns></returns>
        public string GetSendByteStr()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var b in SendByte)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 获取接收字节数组的字符串表示
        /// </summary>
        /// <returns></returns>
        public string GetRecvByteStr()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in RecvByte)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }
            return sb.ToString().Trim();
        }
    }

    public class CommunicationReturnInfo
    {
        public byte[] RecvByte { get; set; }
        public string ExceptionMessage { get; set; }
        public ReturnMessage ReturnValue { get; set; }
    }
}
