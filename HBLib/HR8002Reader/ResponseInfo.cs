using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.HR8002Reader
{
    public class GetReaderInformationInfo : InfoBase
    {
        public byte[] Version { get; set; } = new byte[2]; //版本号
        public byte ReaderType { get; set; } //读卡器类型
        public byte[] ProtocolType { get; set; } = new byte[2]; //支持的协议
        public byte InventoryScanTime { get; set; }  //最大响应时间

        public GetReaderInformationInfo() { }
        public GetReaderInformationInfo(byte[] frame) : base(frame) { }

        /// <summary>
        /// 版本号
        /// </summary>
        public string GetVersionStr()
        {
            return Version[0].ToString().PadLeft(2, '0') +
                "." + Version[1].ToString();
        }

        /// <summary>
        /// 读卡器类型
        /// </summary>
        public string GetReaderTypeStr()
        {
            if (ReaderType == 0x66)
            {
                return "HR8002";
            }
            else
            {
                return "未知型号";
            }
        }

        /// <summary>
        /// 协议类型
        /// </summary>
        /// <returns></returns>
        public string GetProtocolTypeStr()
        {
            StringBuilder sb = new StringBuilder();
            if ((ProtocolType[1] & 0x08) != 0)
            {
                sb.Append("ISO15693");
            }
            if ((ProtocolType[1] & 0x04) != 0)
            {
                if (sb.Length != 0)
                    sb.Append("，");
                sb.Append("ISO14443A");
            }
            if ((ProtocolType[1] & 0x02) != 0)
            {
                if (sb.Length != 0)
                    sb.Append("，");
                sb.Append("ISO14443B");
            }
            return sb.ToString();
        }

        public TimeSpan GetInventoryScanTime()
        {
            return TimeSpan.FromMilliseconds(InventoryScanTime * 100);
        }
    }
}
