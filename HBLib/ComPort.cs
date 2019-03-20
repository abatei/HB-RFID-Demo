using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;

namespace HBLib
{
    public class ComPort : IPort
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; } = 19200;
        public int Timeout { get; set; } = 1000;

        private SerialPort com = new SerialPort();

        public ComPort(string portName, int baudRate, int timeout)
        {
            PortName = portName;
            BaudRate = baudRate;
            Timeout = timeout;
        }

        public ReturnMessage Open()
        {
            com.PortName = PortName;
            com.BaudRate = BaudRate;
            try
            {
                com.Open();
            }
            catch (Exception)
            {
                return ReturnMessage.CP_OpenPortFailed;
            }
            return ReturnMessage.Success;
        }

        public void Close()
        {
            com.Close();
        }

        public async Task SendWithoutAnswerAsync(byte[] data)
        {
            if (!com.IsOpen)
            {
                return;
            }
            try
            {
                await com.BaseStream.WriteAsync(data, 0, data.Length);
            }
            catch(Exception)
            {
                return;
            }
        }

        public async Task<CommunicationReturnInfo> SendAsync(byte[] data)
        {
            CommunicationReturnInfo info = new CommunicationReturnInfo();
            if (!com.IsOpen)
            {
                info.ReturnValue = ReturnMessage.CP_PortHasClosed;
                return info;
            }
            
            //向读写器发送数据
            try
            {
                com.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                info.ReturnValue = ReturnMessage.CP_WriteFaild;
                info.ExceptionMessage = e.Message;
                return info;
            }

            //等待读写器的响应
            CancellationTokenSource cts = new CancellationTokenSource(Timeout);
            try
            {
                byte[] buff = new byte[64];
                int count = 0;
                int offset = 0;
                int temp = await com.BaseStream.ReadAsync(buff, offset, buff.Length - offset, cts.Token);
                while (true)
                {
                    count += temp; 
                    offset += temp;
                    if (buff[0] + 1 > count)
                    {
                        temp = await com.BaseStream.ReadAsync(buff, offset, buff.Length - offset, cts.Token);
                    }
                    else
                    {
                        break;
                    }
                }

                info.RecvByte = new byte[count];
                Array.Copy(buff, 0, info.RecvByte, 0, count);
                info.ReturnValue = ReturnMessage.Success;
                return info;
            }
            catch (Exception e)
            {
                if (cts.IsCancellationRequested)
                {
                    info.ReturnValue = ReturnMessage.CP_TimeOutError;
                    info.ExceptionMessage = e.Message;
                    return info;
                }
                info.ReturnValue = ReturnMessage.CP_ReadFaild;
                info.ExceptionMessage = e.Message;
                return info;
            }
        }
    }
}
