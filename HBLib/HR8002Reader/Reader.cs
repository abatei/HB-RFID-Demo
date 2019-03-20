using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HBLib.HR8002Reader
{
    public class Reader : FrameHelper
    {
        private ComPort com;

        public Reader(byte com_adr, ComPort com) : base(com_adr)
        {
            this.com = com;
        }

        /// <summary>
        /// 上位机发送该命令以获得读写器的信息，包括读写器地址（Com_adr）、
        /// 读写器软件版本（Version）、读写器类型代码、读写器协议支持信息和
        /// InventoryScanTime的信息。
        /// </summary>
        /// <param name="addr">读写器地址</param>
        /// <returns></returns>
        public async Task<GetReaderInformationInfo> GetReaderInformationAsync(byte? addr)
        {
            byte[] frame = CreateGetReaderInformationFrame(addr ?? Com_adr);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                GetReaderInformationInfo gii = new GetReaderInformationInfo();
                gii.SendByte = frame;
                gii.ReturnValue = cri.ReturnValue;
                gii.ExceptionMessage = cri.ExceptionMessage;
                return gii;
            }

            GetReaderInformationInfo info = HandleGetReaderInformationFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机发送该命令关闭读写器的感应场。在感应场关闭情况下，
        /// 读写器不执行ISO15693和ISO14443A/B协议命令。
        /// </summary>
        /// <returns></returns>
        public async Task<InfoBase> CloseRFAsync()
        {
            byte[] frame = CreateCloseRFFrame();
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
        /// 上位机发送该命令开启读写器的感应场。读写器上电后，感应场处于打开状态。
        /// </summary>
        /// <returns></returns>
        public async Task<InfoBase> OpenRFAsync()
        {
            byte[] frame = CreateOpenRFFrame();
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
        /// 上位机发送该命令设置读写器地址为用户给定的值，该值将被写入EEPROM，
        /// 以后将使用此项新的读写器地址。出厂时缺省值是0x00。
        /// 允许用户的修改范围是0x00~0xfe。当用户写入的值是0xff时，
        /// 读写器将会自动恢复成缺省值0x00。
        /// </summary>
        /// <param name="addr">新的读写器地址</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteComAdrAsync(byte addr)
        {
            byte[] frame = CreateWriteComAdrFrame(addr);
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
            Com_adr = addr;
            return info;
        }

        /// <summary>
        /// 上位机发送该命令设置读写器询查命令最大响应时间为用户给定值（3*100ms~255*100ms），
        /// 该值将被写入EEPROM，以后将使用此项新的询查命令最大响应时间。
        /// 出厂时缺省值是0x1e（对应的时间为30*100ms）。用户修改范围是0x03~0xff（
        /// 对应时间是3*100ms~255*100ms）。注意，实际的响应时间可能会比设定值大0~75ms。
        /// 当用户写入的值是0x00~0x02时，读写器将会自动恢复成缺省值0x1e（对应的时间为30*100ms）。
        /// </summary>
        /// <param name="inventoryScanTime">新的最大响应时间</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteInventoryScanTimeAsync(byte inventoryScanTime)
        {
            byte[] frame = CreateWriteInventoryScanTimeFrame(inventoryScanTime);
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
        /// 设置读写器为ISO14443A模式。读写器只有在ISO14443A模式下才能执行ISO14443A协议命令。
        /// </summary>
        /// <returns></returns>
        public async Task<InfoBase> ChangeToISO14443AAsync()
        {
            byte[] frame = CreateChangeToISO14443AFrame();
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
        /// 设置读写器为ISO14443B模式。读写器只有在ISO14443B模式下才能执行ISO14443B协议命令。
        /// </summary>
        /// <returns></returns>
        public async Task<InfoBase> ChangeToISO14443BAsync()
        {
            byte[] frame = CreateChangeToISO14443BFrame();
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
        /// 设置读写器为ISO15693模式。读写器只有在ISO15693模式下才能执行
        /// ISO15693协议命令。读写器上电复位后缺省工作模式为ISO15693模式。
        /// </summary>
        /// <returns></returns>
        public async Task<InfoBase> ChangeToISO15693Async()
        {
            byte[] frame = CreateChangeToISO15693Frame();
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
        /// 上位机发送该命令以控制读写器LED的状态（LED的亮灭持续时间以及闪烁次数）。
        /// </summary>
        /// <param name="duration">持续时间 *50ms</param>
        /// <param name="interval">时间间隔 *50ms</param>
        /// <param name="repeat">重复次数</param>
        /// <returns></returns>
        public async Task<InfoBase> SetLEDAsync(byte duration, byte interval, byte repeat)
        {
            byte[] frame = CreateSetLEDFrame(duration, interval, repeat);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            //读写器让 LED 闪烁是需要占用 MCU 资源的，闪烁花掉多少时间这里
            //就需要休眠多少时间，之后读写器才可以接收后续指令
            Thread.Sleep((duration * 50 + interval * 50) * repeat);
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 上位机发送该命令以控制读写器LED的状态（LED的亮灭持续时间以及闪烁次数）。
        /// </summary>
        /// <param name="duration">持续时间 *50ms</param>
        /// <param name="interval">时间间隔 *50ms</param>
        /// <param name="repeat">重复次数</param>
        /// <returns></returns>
        public async Task<InfoBase> BeepAsync(byte duration, byte interval, byte repeat)
        {
            byte[] frame = CreateBeepFrame(duration, interval, repeat);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InfoBase ib = new InfoBase();
                ib.SendByte = frame;
                ib.ReturnValue = cri.ReturnValue;
                ib.ExceptionMessage = cri.ExceptionMessage;
                return ib;
            }
            //读写器让蜂鸣器发声是需要占用 MCU 资源的，鸣叫花掉多少时间这里
            //就需要休眠多少时间，之后读写器才可以接收后续指令
            Thread.Sleep((duration * 50 + interval * 50) * repeat);
            InfoBase info = HandleBaseFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }
    }
}
