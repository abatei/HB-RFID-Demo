using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib.ISO180006C
{
    public class FrameHelper : HelperBase
    {
        public FrameHelper(byte com_adr) : base(com_adr) { }

        #region 主机向读写器发送指令时，获取所需帧的操作
        protected byte[] CreateInventoryMultiFrame()
        {
            FrameBase frame = new FrameBase(0x04, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.InventoryMulti;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateReadCardFrame(byte ENum, byte[] EPC, MemoryArea Mem,
           byte WordPtr, byte Num, byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte len = (byte)(14 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.ReadCard;
            db[3] = ENum;
            EPC.CopyTo(db, 4);
            int index = 4 + ENum * 2;
            db[index++] = (byte)Mem;
            db[index++] = WordPtr;
            db[index++] = Num;
            Pwd.CopyTo(db, index);
            index += 4;
            db[index++] = MaskAdr;
            db[index] = MaskLen;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateWriteCardFrame(byte WNum, byte ENum, byte[] EPC, MemoryArea Mem,
           byte WordPtr, byte[] Wdt, byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte len = (byte)(14 + WNum * 2 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.WriteCard;
            db[3] = WNum;
            db[4] = ENum;
            EPC.CopyTo(db, 5);
            int index = 5 + ENum * 2;
            db[index++] = (byte)Mem;
            db[index++] = WordPtr;
            Wdt.CopyTo(db, index);
            index += Wdt.Length;
            Pwd.CopyTo(db, index);
            index += 4;
            db[index++] = MaskAdr;
            db[index] = MaskLen;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateWriteEPCFrame(byte ENum, byte[] Pwd, byte[] WEPC)
        {
            byte len = (byte)(9 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.WriteEPC;
            db[3] = ENum;
            Pwd.CopyTo(db, 4);
            WEPC.CopyTo(db, 8);
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateDestroyCardFrame(byte ENum, byte[] EPC,
            byte[] KillPwd, byte MaskAdr, byte MaskLen)
        {
            byte len = (byte)(11 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.DestroyCard;
            db[3] = ENum;
            EPC.CopyTo(db, 4);
            int index = 4 + ENum * 2;
            KillPwd.CopyTo(db, index);
            index += 4;
            db[index++] = MaskAdr;
            db[index] = MaskLen;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetCardProtectFrame(byte ENum, byte[] EPC, ProtectArea Select,
           byte SetProtect, byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte len = (byte)(13 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.SetCardProtect;
            db[3] = ENum;
            EPC.CopyTo(db, 4);
            int index = 4 + ENum * 2;
            db[index++] = (byte)Select;
            db[index++] = SetProtect;
            Pwd.CopyTo(db, index);
            index += 4;
            db[index++] = MaskAdr;
            db[index] = MaskLen;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateEraseCardFrame(byte ENum, byte[] EPC, MemoryArea Mem,
           byte WordPtr, byte Num, byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte len = (byte)(14 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.EraseCard;
            db[3] = ENum;
            EPC.CopyTo(db, 4);
            int index = 4 + ENum * 2;
            db[index++] = (byte)Mem;
            db[index++] = WordPtr;
            db[index++] = Num;
            Pwd.CopyTo(db, index);
            index += 4;
            db[index++] = MaskAdr;
            db[index] = MaskLen;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetReadProtect_G2Frame(byte ENum, byte[] EPC,
            byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte len = (byte)(11 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.SetReadProtect_G2;
            db[3] = ENum;
            EPC.CopyTo(db, 4);
            int index = 4 + ENum * 2;
            Pwd.CopyTo(db, index);
            index += 4;
            db[index++] = MaskAdr;
            db[index] = MaskLen;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetMultiReadProtectFrame(byte[] Pwd)
        {
            FrameBase frame = new FrameBase(0x08, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.SetMultiReadProtect;
            Pwd.CopyTo(db, 3);
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateRemoveReadProtectFrame(byte[] Pwd)
        {
            FrameBase frame = new FrameBase(0x08, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.RemoveReadProtect;
            Pwd.CopyTo(db, 3);
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateCheckReadProtectedFrame()
        {
            FrameBase frame = new FrameBase(0x04, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.CheckReadProtected;

            frame.PushCRC();
            return db;
        }

        protected byte[] CreateSetEASAlarmFrame(byte ENum, byte[] EPC,
            byte[] Pwd, byte EAS, byte MaskAdr, byte MaskLen)
        {
            byte len = (byte)(12 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.SetEASAlarm;
            db[3] = ENum;
            EPC.CopyTo(db, 4);
            int index = 4 + ENum * 2;
            Pwd.CopyTo(db, index);
            index += 4;
            db[index++] = EAS;
            db[index++] = MaskAdr;
            db[index] = MaskLen;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateCheckEASAlarmFrame()
        {
            FrameBase frame = new FrameBase(0x04, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.CheckEASAlarm;

            frame.PushCRC();
            return db;
        }

        protected byte[] CreateLockUserBlockFrame(byte ENum, byte[] EPC,
            byte[] Pwd, byte WrdPointer, byte MaskAdr, byte MaskLen)
        {
            byte len = (byte)(12 + ENum * 2);
            FrameBase frame = new FrameBase(len, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.LockUserBlock;
            db[3] = ENum;
            EPC.CopyTo(db, 4);
            int index = 4 + ENum * 2;
            Pwd.CopyTo(db, index);
            index += 4;
            db[index++] = WrdPointer;
            db[index++] = MaskAdr;
            db[index] = MaskLen;
            frame.PushCRC();
            return db;
        }

        protected byte[] CreateInventorySingleFrame()
        {
            FrameBase frame = new FrameBase(0x04, Com_adr);
            byte[] db = frame.DataBlock;
            db[2] = I180006CCmd.InventorySingle;

            frame.PushCRC();
            return db;
        }
        #endregion

        #region 主机收到读写器发来的帧时，处理这些帧
        protected InventoryInfo HandleInventoryFrame(byte[] frame)
        {
            InventoryInfo info = new InventoryInfo(frame);

            //由于查询成功可能会返回其它的 Status 值，所以帧检查需要单独写
            //如果帧长度不对，则检查不通过
            if (frame[0] != frame.Length - 1)
            {
                info.ReturnValue = ReturnMessage.HF_FrameLenError;
                return info;
            }
            //检查读写器地址
            if (frame[1] != Com_adr && frame[1] != 0xFF)
            {
                info.ReturnValue = ReturnMessage.HF_ReaderAddrError;
                return info;
            }
            //CRC校验
            UInt16 crc = FrameBase.GetCRC(frame);
            UInt16 a = BitConverter.ToUInt16(frame, frame.Length - 2);
            UInt16 b = FrameBase.Reverse(crc);
            if (BitConverter.ToUInt16(frame, frame.Length - 2) != crc)
            {
                info.ReturnValue = ReturnMessage.HF_CRCError;
                return info;
            }
            //压入结果状态码
            info.SetStatus();
            if (info.Status > 0x04)
            {
                info.ReturnValue = ReturnMessage.HF_StatusError;
                return info;
            }

            info.Num = frame[4];
            var epcSet = info.EPCSet;
            int index = 5;
            for (int i = 0; i < info.Num; i++)
            {
                InventoryInfo.EPCInfo epc = new InventoryInfo.EPCInfo();
                int len = frame[index++];
                epc.EPCLen = (byte)(len / 2);
                epc.EPC = new byte[len];
                Array.Copy(frame, index, epc.EPC, 0, len);
                epcSet.Add(epc);
                index += len;
            }

            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected ReadCardInfo HandleReadCardFrame(byte[] frame)
        {
            ReadCardInfo info = new ReadCardInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }

            int len = frame.Length - 6;
            info.Data = new byte[len];
            Array.Copy(frame, 4, info.Data, 0, info.Data.Length); //拷贝数据

            info.ReturnValue = ReturnMessage.Success;
            return info;
        }

        protected CheckReadProtectedInfo HandelCheckReadProtectedFrame(byte[] frame)
        {
            CheckReadProtectedInfo info = new CheckReadProtectedInfo(frame);
            ReturnMessage returnCode = CheckFrame(info);
            if (returnCode != ReturnMessage.Success)
            {
                info.ReturnValue = returnCode;
                return info;
            }
            info.Protected = frame[4] == 0x00 ? false : true;

            info.ReturnValue = ReturnMessage.Success;
            return info;
        }
        #endregion
    }
}
