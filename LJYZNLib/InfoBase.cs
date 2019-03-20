using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib
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
            Status = RecvByte[3];
            if (Status == 0xFC)
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
                case 0xFC: //EPC C1G2（ISO18000 -6C）电子标签错误代码
                    sb.Append("：");
                    sb.Append(CodeInterpret.G2ErrorSet[ErrorCode]);
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
