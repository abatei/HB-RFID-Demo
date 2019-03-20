using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib
{
    /// <summary>
    /// 常量状态码（Status）解释字符串
    /// </summary>
    public static class CodeInterpret
    {
        public static SortedList<ReturnMessage, string> ReturnMessageSet = new SortedList<ReturnMessage, string>()
        {
            [ReturnMessage.Success] = "操作成功！",
            [ReturnMessage.HF_FrameLenError] = "帧长度错误！",
            [ReturnMessage.HF_ReaderAddrError] = "读写器地址不匹配！",
            [ReturnMessage.HF_CRCError] = "接收消息的CRC效验错误！",
            [ReturnMessage.HF_StatusError] = "错误请参考状态码",
            [ReturnMessage.CP_OpenPortFailed] = "通信端口打开出错！",
            [ReturnMessage.CP_PortHasClosed] = "通信端口已经关闭！",
            [ReturnMessage.CP_TimeOutError] = "向读写器发送消息后，等待返回的消息超时！",
            [ReturnMessage.CP_ReadFaild] = "接收消息出错！",
            [ReturnMessage.CP_WriteFaild] = "发送消息出错"
        };

        public static SortedList<FrequencyBand, string> BandCountrySet = new SortedList<FrequencyBand, string>()
        {
            [FrequencyBand.UserBand] = "User Band",
            [FrequencyBand.ChineseBand2] = "Chinese band2",
            [FrequencyBand.USBand] = "US band",
            [FrequencyBand.KoreanBand] = "Korean band",
            [FrequencyBand.EUBand] = "EU band"
        };

        public static SortedList<byte, string> StatusSet = new SortedList<byte, string>()
        {
            [0x00] = "操作成功！",
            [0x01] = "G2-询查时间结束前返回询查结果！",
            [0x02] = "G2-指定的询查时间溢出时，读写器还未完成询查操作！",
            [0x03] = "G2-本条消息之后，还有消息！",
            [0x04] = "G2-读写器存储空间已满！",
            [0x05] = "访问密码错误！",
            [0x09] = "G2-销毁标签失败！",
            [0x0A] = "销毁密码不能为全0！",
            [0x0B] = "G2-电子标签不支持该命令！",
            [0x0C] = "NXP UCODE EPC G2X-对该命令访问密码不能为全0！",
            [0x0D] = "NXP UCODE EPC G2X-电子标签已经被设置了读保护，不能再次设置！",
            [0x0E] = "NXP UCODE EPC G2X-电子标签没有被设置读保护，不需要解锁！",
            [0x10] = "6B-有字节空间被锁定，写入失败！",
            [0x11] = "6B-不能锁定！",
            [0x12] = "6B-已经锁定，不能再次锁定！",
            [0x13] = "参数保存失败，但设置的值在读写器断电前有效！",
            [0x14] = "无法调整功率！",
            [0x15] = "6B-在询查时间结束前返回！",
            [0x16] = "6B-在指定的询查时间溢出时，读写器还未完成询查操作！！",
            [0x17] = "6B-本条消息之后，还有消息！",
            [0x18] = "6B-读写器存储空间已满！",
            [0x19] = "电子标签不支持该命令或者访问密码不能为0！",
            [0xF9] = "命令执行出错！",
            [0xFA] = "有电子标签，但通信不畅，操作失败！",
            [0xFB] = "无电子标签可操作！",
            [0xFC] = "电子标签返回错误代码！",
            [0xFD] = "命令长度错误！",
            [0xFE] = "不合法的命令！",
            [0xFF] = "参数错误！",
        };

        public static SortedList<byte, string> G2ErrorSet = new SortedList<byte, string>()
        {
            [0x00] = "其它错误。",
            [0x03] = "存储位置不存在或标签不支持的PC值。",
            [0x04] = "存储位置锁定或永久锁定，且不可写入。",
            [0x0B] = "标签电源不足，无法执行存储写入操作。",
            [0x0F] = "标签不支持特定错误代码。",

        };
    }

    public enum FrequencyBand : byte
    {
        UserBand = 0x00,
        ChineseBand2 = 0x01,
        USBand = 0x02,
        KoreanBand = 0x03,
        EUBand = 0x04
    }

    //读写器频率转换器
    public static class FrequencyTranslater
    {
        class BandParameter
        {
            //Fs = 902.6 + N * 0.4 (MHz) 其中N∈[0, 62]。
            public FrequencyBand Country { get; set; } //国家名称
            public double MinFrequency { get; set; } //频率最小值
            public int IntervalCount { get; set; } //N值
            public double Interval { get; set; } //间隔
        }

        //此数组用于存放每个国家所使用频段的参数
        static BandParameter[] band =
         {
            new BandParameter()
            {
                Country = FrequencyBand.UserBand,
                MinFrequency = 902.6,
                IntervalCount = 62,
                Interval = 0.4
            },
            new BandParameter()
            {
                Country = FrequencyBand.ChineseBand2,
                MinFrequency = 920.125,
                IntervalCount = 19,
                Interval = 0.25
            },
            new BandParameter()
            {
                Country = FrequencyBand.USBand,
                MinFrequency = 902.75,
                IntervalCount = 49,
                Interval = 0.5
            },
            new BandParameter()
            {
                Country = FrequencyBand.KoreanBand,
                MinFrequency = 917.1,
                IntervalCount = 31,
                Interval = 0.2
            },
            new BandParameter()
            {
                Country = FrequencyBand.EUBand,
                MinFrequency = 865.1,
                IntervalCount = 14,
                Interval = 0.2
            }
        };

        /// <summary>
        ///  返回指定国家频段的最小值
        /// </summary>
        /// <param name="country">国家</param>
        /// <returns></returns>
        public static double GetMinFrequency(FrequencyBand country)
        {
            return band.Where(c => c.Country == country).First().MinFrequency;
        }

        /// <summary>
        /// 返回指定国家频段的最大值
        /// </summary>
        /// <param name="country">国家</param>
        /// <returns></returns>
        public static double GetMaxFrequency(FrequencyBand country)
        {
            BandParameter para = band.Where(c => c.Country == country).First();
            double minFre = para.MinFrequency;
            double intervalCount = para.IntervalCount;
            double interval = para.Interval;
            return minFre + intervalCount * interval;
        }

        /// <summary>
        /// 获取频段所属国家
        /// </summary>
        /// <param name="MaxFre">表示最小频率的一个字节</param>
        /// <param name="MinFre">表示最大频率的一个字节</param>
        /// <returns></returns>
        public static string GetBandCountry(byte MinFre, byte MaxFre)
        {
            byte bandByte = (byte)((MaxFre & 0xC0) >> 4 | MinFre >> 6);
            FrequencyBand country = (FrequencyBand)bandByte;
            return CodeInterpret.BandCountrySet[country];
        }

        /// <summary>
        /// 获取频率范围，用于设置读写器工作频率，必须两个参数同时使用才能知晓是哪个频段
        /// </summary>
        /// <param name="MaxFre">表示最小频率的一个字节</param>
        /// <param name="MinFre">表示最大频率的一个字节</param>
        /// <returns></returns>
        public static Tuple<double, double> GetFrequencyRange(byte MinFre, byte MaxFre)
        {
            byte bandByte = (byte)((MaxFre & 0xC0) >> 4 | MinFre >> 6);
            FrequencyBand country = (FrequencyBand)bandByte;
            int max = MaxFre & 0x3F;
            int min = MinFre & 0x3F;
            BandParameter para = band.Where(c => c.Country == country).First();
            double maxF = para.MinFrequency + max * para.Interval;
            double minF = para.MinFrequency + min * para.Interval;
            return Tuple.Create(minF, maxF);
        }
    }

    /// <summary>
    /// 返回消息的枚举，其中：
    /// HF_ 前缀为处理帧时遇到的错误
    /// CP_ 前缀为与读写器进行通信时遇到的错误，收到此类错误表示未收到读写器返回的信息
    /// </summary>
    public enum ReturnMessage : byte
    {
        Success = 0x00,
        HF_FrameLenError = 0x10,
        HF_ReaderAddrError = 0x11,
        HF_CRCError = 0x12,
        HF_StatusError = 0x13,
        CP_OpenPortFailed = 0x20,
        CP_PortHasClosed = 0x21,
        CP_TimeOutError = 0x22,
        CP_ReadFaild = 0x23,
        CP_WriteFaild = 0x24
    }
}
