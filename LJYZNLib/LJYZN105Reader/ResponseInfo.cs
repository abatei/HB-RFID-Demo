using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib.LJYZN105Reader
{
    public class GetInformationInfo : InfoBase
    {
        public byte[] Version { get; set; } = new byte[2]; //版本号
        public byte Type { get; set; } //读写器类型
        public byte TrType { get; set; } //读写器支持的协议信息
        public byte Dmaxfre { get; set; } //当前读写器工作的最大频率
        public byte Dminfre { get; set; } //当前读写器工作的最小频率
        public byte Power { get; set; } //读写器的输出功率。范围是0到13
        public byte Scntm { get; set; } //询查时间。读写器收到询查命令后，在询查时间内，会给上位机应答。

        public GetInformationInfo() { }
        public GetInformationInfo(byte[] frame) : base(frame) { }

        /// <summary>
        /// 获取读写器版本号
        /// </summary>
        /// <returns></returns>
        public string GetVersionStr()
        {
            return Version[0].ToString().PadLeft(2, '0') +
                "." + Version[1].ToString();
        }

        /// <summary>
        /// 读写器类型
        /// </summary>
        public string GetReaderTypeStr()
        {
            if (Type == 0x08)
            {
                return "UHFReader09";
            }
            else
            {
                return "未知型号";
            }
        }

        /// <summary>
        /// 支持协议类型
        /// </summary>
        /// <returns></returns>
        public string GetProtocolTypeStr()
        {
            StringBuilder sb = new StringBuilder();
            if ((TrType & 0x01) != 0)
            {
                sb.Append("18000-6B");
            }
            if ((TrType & 0x02) != 0)
            {
                if (sb.Length != 0)
                    sb.Append("，");
                sb.Append("18000-6C");
            }
            return sb.ToString();
        }

        public string GetBandCountry()
        {
            return FrequencyTranslater.GetBandCountry(Dminfre, Dmaxfre);
        }

        public string GetFrequencyRange()
        {
            var range = FrequencyTranslater.GetFrequencyRange(Dminfre, Dmaxfre);
            return range.Item1.ToString() + "~" + range.Item2.ToString();
        }
    }
}
