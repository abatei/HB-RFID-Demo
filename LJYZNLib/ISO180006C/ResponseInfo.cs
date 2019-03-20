using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib.ISO180006C
{
    public class InventoryInfo : InfoBase
    {
        public class EPCInfo
        {
            public byte EPCLen { get; set; }
            public byte[] EPC { get; set; }

            public string GetEPCStr()
            {
                return FrameBase.UIDToString(EPC);
            }
        }

        public byte Num { get; set; }
        public List<EPCInfo> EPCSet { get; set; } = new List<EPCInfo>();

        public InventoryInfo() { }
        public InventoryInfo(byte[] frame) : base(frame) { }
    }

    public class ReadCardInfo : InfoBase
    {
        public byte[] Data { get; set; }

        public ReadCardInfo() { }
        public ReadCardInfo(byte[] frame) : base(frame) { }

        public string GetDataStr()
        {
            return FrameBase.ByteSetToString(Data);
        }
    }

    public class CheckReadProtectedInfo : InfoBase
    {
        public bool Protected { get; set; }

        public CheckReadProtectedInfo() { }
        public CheckReadProtectedInfo(byte[] frame) : base(frame) { }
    }
}
