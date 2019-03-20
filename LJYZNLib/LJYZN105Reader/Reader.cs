using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib.LJYZN105Reader
{
    public class Reader : FrameHelper
    {
        private ComPort com;

        public Reader(byte com_adr, ComPort com) : base(com_adr)
        {
            this.com = com;
        }

        /// <summary>
        /// 当上位机通过发送命令数据块让读写器执行该命令后，将获得读写器的信息，
        /// 这其中包括读写器地址（Adr）、读写器软件版本（Version）、读写器类型
        /// 代码、读写器协议支持信息、读写器的频率范围、读写器的功率、询查时间
        /// 等信息。
        /// </summary>
        /// <returns></returns>
        public async Task<GetInformationInfo> GetInformationAsync()
        {
            byte[] frame = CreateGetInformationFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                GetInformationInfo gii = new GetInformationInfo();
                gii.SendByte = frame;
                gii.ReturnValue = cri.ReturnValue;
                gii.ExceptionMessage = cri.ExceptionMessage;
                return gii;
            }

            GetInformationInfo info = HandleGetInformationFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 这个命令用来选择频段及各频段中的上限频率，下限频率。上限频率必须大于或等于下限频率。
        /// </summary>
        /// <param name="minFre">一个字节，Bit7-Bit6用于频段设置用；Bit5-Bit0表示读写器工作的最大频率</param>
        /// <param name="maxFre">一个字节，Bit7-Bit6用于频段设置用；Bit5-Bit0表示读写器工作的最小频率。最小频率必须小于等于最大频率。</param>
        /// <returns></returns>
        public async Task<InfoBase> SetFrequencyAsync(byte minFre, byte maxFre)
        {
            byte[] frame = CreateSetFrequencyFrame(minFre, maxFre);
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
        /// 设置读写器地址。本条命令使用原来的地址应答。
        /// </summary>
        /// <param name="address">要设置的新的读写器地址这个地址不能为0xFF。如果设置为0xFF，则读写器将返回参数出错信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> SetAddrAsync(byte address)
        {
            byte[] frame = CreateSetAddrFrame(address);
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
            Com_adr = address;
            return info;
        }

        /// <summary>
        /// 设置读写器询查时间。读写器将会把询查命令最大响应时间改为用户给定的值
        /// （3*100ms~255*100ms），以后将使用此项新的询查命令最大响应时间。出厂时
        /// 缺省值是0x0a（对应的时间为10*100ms）。用户修改范围是0x03~0xff（对应时
        /// 间是3*100ms~255*100ms）。注意，实际的响应时间可能会比设定值大0~75ms。
        /// 当用户写入的值是0x00~0x02 时，读写器将会自动恢复成缺省值 0x0a（对应的
        /// 时间为10*100ms）。
        /// </summary>
        /// <param name="scanTime">询查时间，范围是0x03~0xff（对应时间是3*100ms~255*100ms）</param>
        /// <returns></returns>
        public async Task<InfoBase> SetInventoryTimeAsync(byte scanTime)
        {
            byte[] frame = CreateSetInventoryTimeFrame(scanTime);
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
        /// 设置串口波特率
        /// </summary>
        /// <param name="baudRate">新的波特率。波特率默认为57600。BaudRate的范围是0 ~ 6。其它值保留。
        /// 其对应的波特率为：
        /// 0：9600bps
        /// 1：19200 bps
        /// 2：38400 bps
        /// 3：43000 bps
        /// 4：56000 bps
        /// 5：57600 bps
        /// 6：115200 bps
        /// </param>
        /// <returns></returns>
        public async Task<InfoBase> SetBaudRateAsync(byte baudRate)
        {
            byte[] frame = CreateSetBaudRateFrame(baudRate);
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
        /// 调整功率
        /// </summary>
        /// <param name="pwr">要设定的功率参数。范围是0~13。</param>
        /// <returns></returns>
        public async Task<InfoBase> SetPowerDbmAsync(byte pwr)
        {
            byte[] frame = CreateSetPowerDbmFrame(pwr);
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
        /// 该命令用来控制LED灯和蜂鸣器按一定规律闪烁和鸣叫。
        /// </summary>
        /// <param name="activeT">LED灯亮和蜂鸣器鸣叫时间(ActiveT*50ms)，默认值为零。0 小于等于 ActiveT 小于等于 255。</param>
        /// <param name="silentT">LED灯和蜂鸣器静默时间(SilentT *50ms)，默认值为零。0 小于等于 SilentT 小于等于 255。</param>
        /// <param name="times">LED灯亮和蜂鸣器鸣叫次数(0 小于等于 Times 小于等于 255) 默认值为零。</param>
        /// <returns></returns>
        public async Task<InfoBase> BuzzerAndLEDControlAsync(byte activeT, byte silentT, byte times)
        {
            byte[] frame = CreateBuzzerAndLEDControlFrame(activeT, silentT, times);
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
