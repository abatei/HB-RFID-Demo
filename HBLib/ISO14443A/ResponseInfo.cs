using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBLib.ISO14443A
{
    public class RequestInfo : InfoBase
    {
        /// <summary>
        /// 返回的卡类型
        /// </summary>
        public byte[] CardType { get; set; } = new byte[2];

        public RequestInfo() { }
        public RequestInfo(byte[] frame) : base(frame) { }

        /// <summary>
        /// 获取卡类型的数字显示形式
        /// </summary>
        public string GetCardTypeNum()
        {
            return CardType[0].ToString("X2") + CardType[1].ToString("X2");
        }

        /// <summary>
        /// 获取卡类型名称
        /// </summary>
        /// <returns></returns>
        public string GetCardTypeName()
        {
            string str = "未知类型";
            switch (GetCardTypeNum())
            {
                case "0400":
                    str = "Mifare S50";
                    break;
                case "0200":
                    str = "Mifare S70";
                    break;
            }
            return str;
        }
    }

    public class AnticollInfo : InfoBase
    {
        /// <summary>
        /// 返回的电子标签序列号
        /// </summary>
        public byte[] UID { get; set; } = new byte[4];

        public AnticollInfo() { }
        public AnticollInfo(byte[] frame) : base(frame) { }

        /// <summary>
        /// 返回电子标签序列号的字符串形式
        /// </summary>
        /// <returns></returns>
        public string GetUIDStr()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in UID)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }

    public class ULAnticollInfo : InfoBase
    {
        public byte[] UL_UID { get; set; } = new byte[7];

        public ULAnticollInfo() { }
        public ULAnticollInfo(byte[] frame) : base(frame) { }

        /// <summary>
        /// 返回电子标签序列号的字符串形式
        /// </summary>
        /// <returns></returns>
        public string GetUL_UIDStr()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in UL_UID)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }

    public class SelectInfo : InfoBase
    {
        public byte Size { get; set; }

        public SelectInfo() { }
        public SelectInfo(byte[] frame) : base(frame) { }
    }

    public class ReadInfo : InfoBase
    {
        public byte[] BlockData { get; set; } = new byte[16];

        public ReadInfo() { }
        public ReadInfo(byte[] frame) : base(frame) { }

        public string GetBlockDataStr()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in BlockData)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }
            return sb.ToString().Trim();
        }
    }

    public class ReadValueInfo : InfoBase
    {
        public byte[] ValueData { get; set; } = new byte[4];

        public ReadValueInfo() { }
        public ReadValueInfo(byte[] frame) : base(frame) { }

        public string GetValueDataStr()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in ValueData)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }
            return sb.ToString().Trim();
        }
    }

    public class ReadE2Info : InfoBase
    {
        public byte[] E2Data { get; set; }

        public ReadE2Info() { }
        public ReadE2Info(byte[] frame) : base(frame)
        {
            if (frame[0] > 4)
            {
                E2Data = new byte[frame[0] - 4];
            }
        }

        public string GetE2DataStr()
        {
            if (E2Data == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var b in E2Data)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }
            return sb.ToString().Trim();
        }
    }
}
