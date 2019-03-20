using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib.ISO180006C
{
    public class I180006C : FrameHelper
    {
        private ComPort com;

        public I180006C(byte com_adr, ComPort com) : base(com_adr)
        {
            this.com = com;
        }

        /// <summary>
        /// 询查多标签。
        /// 询查命令的作用是检查有效范围内是否有符合协议的电子标签存在。想要对未知EPC的新标签
        /// 进行别的操作前，应先通过询查命令来得到标签的EPC号。
        /// 在运行询查命令之前，用户可以根据需要先设定好该命令的最大运行时间(询查时间)。读写
        /// 器在询查时间规定的范围内必须给上位机一个结果，如果读写器尚未读完有效范围内的所有
        /// 标签，而询查时间已到，则读写器不再询查其它标签，而是直接把已经询查到得标签返回给
        /// 上位机，并提示上位机还有标签未读完。然后等待下一个命令。
        /// 询查时间的缺省值是1s，用户可以通过运行读写器自定义命令设定询查时间命令来修改。允
        /// 许的范围是：3*100ms ~255*100ms(实际的响应时间可能会比设定的值大0~75ms)。询查时间
        /// 如果设定的过短，可能会出现在规定时间内询查不到电子标签的情况。
        /// </summary>
        /// <returns></returns>
        public async Task<InventoryInfo> InventoryMultiAsync()
        {
            byte[] frame = CreateInventoryMultiFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InventoryInfo ii = new InventoryInfo();
                ii.SendByte = frame;
                ii.ReturnValue = cri.ReturnValue;
                ii.ExceptionMessage = cri.ExceptionMessage;
                return ii;
            }

            InventoryInfo info = HandleInventoryFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 询查单张标签
        /// </summary>
        /// <returns></returns>
        public async Task<InventoryInfo> InventorySingleAsync()
        {
            byte[] frame = CreateInventorySingleFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                InventoryInfo ii = new InventoryInfo();
                ii.SendByte = frame;
                ii.ReturnValue = cri.ReturnValue;
                ii.ExceptionMessage = cri.ExceptionMessage;
                return ii;
            }

            InventoryInfo info = HandleInventoryFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 这个命令读取标签的保留区、EPC存储区、TID存储区或用户存储区中的数据。从指定的地址开始读，以字为单位。
        /// </summary>
        /// <param name="ENum">EPC号长度，以字为单位。EPC的长度在15个字以内，可以为0。超出范围，将返回参数错误信息。</param>
        /// <param name="EPC">要读取数据的标签的EPC号。长度根据所给的EPC号决定，EPC号以字为单位，且必须是整数个长度。高字在前，每个字的高字节在前。这里要求给出的是完整的EPC号。</param>
        /// <param name="Mem">一个字节。选择要读取的存储区。0x00：保留区；0x01：EPC存储区；0x02：TID存储区；0x03：用户存储区。其他值保留。若命令中出现了其它值，将返回参数出错的消息。</param>
        /// <param name="WordPtr">一个字节。指定要读取的字起始地址。0x00 表示从第一个字(第一个16位存储区)开始读，0x01表示从第2个字开始读，依次类推。</param>
        /// <param name="Num">一个字节。要读取的字的个数。不能设置为0x00，否则将返回参数错误信息。Num不能超过120，即最多读取120个字。若Num设置为0或者超过了120，将返回参数出错的消息。</param>
        /// <param name="Pwd">四个字节，这四个字节是访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。只有当读保留区，并且相应存储区设置为密码锁、且标签的访问密码为非0的时候，才需要使用正确的访问密码。在其他情况下，Pwd为零或正确的访问密码。</param>
        /// <param name="MaskAdr">一个字节，掩模EPC号的起始字节地址。0x00表示从EPC号的最高字节开始掩模，0x01表示从EPC号的第二字节开始掩模，以此类推。</param>
        /// <param name="MaskLen">一个字节，掩模的字节数。掩模起始字节地址+掩模字节数不能大于EPC号字节长度，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<ReadCardInfo> ReadCardAsync(byte ENum, byte[] EPC, MemoryArea Mem,
           byte WordPtr, byte Num, byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte[] frame = CreateReadCardFrame(ENum, EPC, Mem, WordPtr, Num, Pwd, MaskAdr, MaskLen);
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                ReadCardInfo rci = new ReadCardInfo();
                rci.SendByte = frame;
                rci.ReturnValue = cri.ReturnValue;
                rci.ExceptionMessage = cri.ExceptionMessage;
                return rci;
            }

            ReadCardInfo info = HandleReadCardFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 这个命令可以一次性往保留区、TID存储区或用户存储区中写入若干个字。
        /// </summary>
        /// <param name="WNum">待写入的字个数，一个字为2个字节。这里字的个数必须和实际待写入的数据个数相等。WNum必须大于0，若上位机给出的WNum为0或者WNum和实际字个数不相等，将返回参数错误的消息。</param>
        /// <param name="ENum">EPC号长度。以字为单位。EPC的长度在15个字以内，可以为0。否则返回参数错误信息。</param>
        /// <param name="EPC">要写入数据的标签的EPC号。长度由所给的EPC号决定，EPC号以字为单位，且必须是整数个长度。高字在前，每个字的高字节在前。这里要求给出的是完整的EPC号。</param>
        /// <param name="Mem">一个字节，选择要写入的存储区。0x00：保留区；0x02：TID存储区；0x03：用户存储区。其他值保留。若命令中出现了其它值，将返回参数出错的消息。</param>
        /// <param name="WordPtr">一个字节，指定要写入数据的起始地址。</param>
        /// <param name="Wdt">待写入的字，字的个数必须与WNum指定的一致。这是要写入到存储区的数据。每个字的高字节在前。如果给出的数据不是整数个字长度，Data[]中前面的字写在标签的低地址中，后面的字写在标签的高地址中。比如，WordPtr等于0x02，则Data[]中第一个字(从左边起)写在Mem指定的存储区的地址0x02中，第二个字写在0x03中，依次类推。</param>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。在写操作时，应给出正确的访问密码，当相应存储区未设置成密码锁时Pwd可以为零。</param>
        /// <param name="MaskAdr">一个字节，掩模EPC号的起始字节地址。0x00表示从EPC号的最高字节开始掩模，0x01表示从EPC号的第二字节开始掩模，以此类推。</param>
        /// <param name="MaskLen">一个字节，掩模的字节数。掩模起始字节地址+掩模字节数不能大于EPC号字节长度，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteCardAsync(byte WNum, byte ENum, byte[] EPC, MemoryArea Mem,
           byte WordPtr, byte[] Wdt, byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte[] frame = CreateWriteCardFrame(WNum, ENum, EPC, Mem, WordPtr, Wdt, Pwd, MaskAdr, MaskLen);
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
        /// 这个命令向电子标签写入EPC号。写入的时候，天线有效范围内只能有一张电子标签。
        /// </summary>
        /// <param name="ENum">1个字节。要写入的EPC的长度，以字为单位。可以为0，但不能超过15，否则返回参数错误信息。</param>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。在本命令中，当EPC区设置为密码锁、且标签访问密码为非0的时候，才需要使用访问密码。在其他情况下，Pwd为零或正确的访问密码。</param>
        /// <param name="WEPC">要写入的EPC号，长度必须和ENum说明的一样。WEPC最小0个字，最多15个字，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> WriteEPCAsync(byte ENum, byte[] Pwd, byte[] WEPC)
        {
            byte[] frame = CreateWriteEPCFrame(ENum, Pwd, WEPC);
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
        /// 销毁标签。标签销毁后，永远不会再处理读写器的命令。
        /// </summary>
        /// <param name="ENum">EPC号长度，以字为单位。EPC的长度在15个字以内，可以为0，否则返回参数错误信息。</param>
        /// <param name="EPC">要写入数据的标签的EPC号。长度根据所给的EPC号决定，EPC号以字为单位，且必须是整数个长度。高字在前，每个字的高字节在前。这里要求给出的是完整的EPC号。</param>
        /// <param name="KillPwd">4个字节的销毁密码。32位的销毁密码的最高位在Killpwd的第一字节(从左往右)的最高位，销毁密码最低位在Killpwd第四字节的最低位，Killpwd的前两个字节放置销毁密码的高字。要销毁标签，则销毁密码必须为非0，因为密码为0的标签是无法销毁的。如果命令中的销毁密码为0，则返回参数错误的应答。</param>
        /// <param name="MaskAdr">一个字节，掩模EPC号的起始字节地址。0x00表示从EPC号的最高字节开始掩模，0x01表示从EPC号的第二字节开始掩模，以此类推。</param>
        /// <param name="MaskLen">一个字节，掩模的字节数。掩模起始字节地址+掩模字节数不能大于EPC号字节长度，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> DestroyCardAsync(byte ENum, byte[] EPC,
            byte[] KillPwd, byte MaskAdr, byte MaskLen)
        {
            byte[] frame = CreateDestroyCardFrame(ENum, EPC, KillPwd, MaskAdr, MaskLen);
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
        /// 这个命令可以设定保留区为无保护下的可读可写、永远可读可写、带密码可读可
        /// 写、永远不可读不可写；可以分别设定EPC存储区、用户存储区为无保护下的可
        /// 写、永远可写、带密码可写、永远不可写；TID存储区是只读的，永远都不可写。
        /// EPC存储区、TID存储区和用户存储区是永远可读的。
        /// 标签的保留区一旦设置为永远可读写或永远不可读写，则以后不能再更改其读写
        /// 保护设定。标签的EPC存储区、TID存储区或用户存储区若是设置为永远可写或永
        /// 远不可写，则以后不能再更改其读写保护设定。如果强行发命令欲改变以上几种
        /// 状态，则电子标签将返回错误代码。
        /// 在把某个存储区设置为带密码可读写、带密码可写或把带密码锁状态设置为其它
        /// 非密码锁状态时，必须给出访问密码，所以，在进行此操作前，必须确保电子标
        /// 签已设置了访问密码。
        /// </summary>
        /// <param name="ENum">EPC号长度，以字为单位。EPC的长度在15个字以内，可以为0，否则返回参数错误信息。</param>
        /// <param name="EPC">要写入数据的标签的EPC号。长度由所给的EPC号决定，EPC号以字为单位，且必须是整数个长度。高字在前，每个字的高字节在前。这里要求给出的是完整的EPC号。</param>
        /// <param name="Select">一个字节。定义如下：
        ///     Select为0x00时，控制Kill密码读写保护设定。
        ///     Select为0x01时，控制访问密码读写保护设定。
        ///     Select为0x02时，控制EPC存储区读写保护设定。
        ///     Select为0x03时，控制TID存储区读写保护设定。
        ///     Select为0x04时，控制用户存储区读写保护设定。
        /// 其它值保留，若读写器接收到了其他值，将返回参数出错的消息，并且不执行命令。</param>
        /// <param name="SetProtect">SetProtect的值根据Select的值而确定。
        /// 当Select为0x00或0x01，即当设置Kill密码区或访问密码区的时候，SetProtect的值代表的意义如下：
        ///     0x00：设置为无保护下的可读可写
        ///     0x01：设置为永远可读可写
        ///     0x02：设置为带密码可读可写
        ///     0x03：设置为永远不可读不可写
        /// 当Select与SetProtect出现了其他值的时候，将返回参数出错的消息，并且不执行命令。</param>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。必须给出正确的访问密码。</param>
        /// <param name="MaskAdr">一个字节，掩模EPC号的起始字节地址。0x00表示从EPC号的最高字节开始掩模，0x01表示从EPC号的第二字节开始掩模，以此类推。</param>
        /// <param name="MaskLen">一个字节，掩模的字节数。掩模起始字节地址+掩模字节数不能大于EPC号字节长度，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> SetCardProtectAsync(byte ENum, byte[] EPC, ProtectArea Select,
           byte SetProtect, byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte[] frame = CreateSetCardProtectFrame(ENum, EPC, Select, SetProtect, Pwd, MaskAdr, MaskLen);
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
        /// 此命令可以擦除标签的保留区、EPC存储区、TID存储区或用户存储区的若干字。
        /// </summary>
        /// <param name="ENum">EPC号长度。以字为单位。EPC的长度在15个字以内，可以为0，否则返回参数错误信息。</param>
        /// <param name="EPC">要写入数据的标签的EPC号。长度根据所给的EPC号决定，EPC号以字为单位，且必须是整数个长度。高字在前，每个字的高字节在前。这里要求给出的是完整的EPC号。</param>
        /// <param name="Mem">1个字节，选择要读取的存储区。0x00：保留区；0x01：EPC区；0x02：TID存储区；0x03：用户存储区。其他值保留，若命令中出现了其它值，则返回参数错误信息。</param>
        /// <param name="WordPtr">1个字节，指定要擦除的字起始地址。0x00 表示从第一个字(第一个16位存储体)开始擦除，0x01表示从第2个字开始擦除，依次类推。当擦除EPC区时，WordPtr必须大于等于0x01，若小于0x01，则返回参数错误消息。</param>
        /// <param name="Num">1个字节，指定要擦除的字的个数。从WordPtr指定的地址开始擦除，擦除Num指定个数的字。若Num为0x00，则返回参数错误信息。</param>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。当进行擦除操作时，并且相应存储区设置为密码锁的时候，才必须使用正确的访问密码。其它情况下，Pwd为零或正确的访问密码。</param>
        /// <param name="MaskAdr">一个字节，掩模EPC号的起始字节地址。0x00表示从EPC号的最高字节开始掩模，0x01表示从EPC号的第二字节开始掩模，以此类推。</param>
        /// <param name="MaskLen">一个字节，掩模的字节数。掩模起始字节地址+掩模字节数不能大于EPC号字节长度，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> EraseCardAsync(byte ENum, byte[] EPC, MemoryArea Mem,
           byte WordPtr, byte Num, byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte[] frame = CreateEraseCardFrame(ENum, EPC, Mem, WordPtr, Num, Pwd, MaskAdr, MaskLen);
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
        /// 这个命令根据电子标签的EPC号，对标签设置读保护，使得电子标签不能被任何命令
        /// 读写，对标签进行询查操作，也无法得到电子标签的EPC号。仅对NXP UCODE EPC G2X
        /// 标签有效。
        /// </summary>
        /// <param name="ENum">EPC号长度。以字为单位。EPC的长度在15个字以内，可以为0，否则返回参数错误信息。</param>
        /// <param name="EPC">要写入数据的标签的EPC号。长度由所给的EPC号决定，EPC号以字为单位，且必须是整数个长度。高字在前，每个字的高字节在前。这里要求给出的是完整的EPC号。</param>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。待设定读保护的电子标签访问密码必须不为0，访问密码为0 的电子标签是无法设置读保护的，在命令中，必须给出正确的访问密码。</param>
        /// <param name="MaskAdr">一个字节，掩模EPC号的起始字节地址。0x00表示从EPC号的最高字节开始掩模，0x01表示从EPC号的第二字节开始掩模，以此类推</param>
        /// <param name="MaskLen">一个字节，掩模的字节数。掩模起始字节地址+掩模字节数不能大于EPC号字节长度，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> SetReadProtect_G2Async(byte ENum, byte[] EPC,
            byte[] Pwd, byte MaskAdr, byte MaskLen)
        {
            byte[] frame = CreateSetReadProtect_G2Frame(ENum, EPC, Pwd, MaskAdr, MaskLen);
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
        /// 这个命令可以为有效范围内的电子标签设定读保护。这个命令与前面一个命令的区别是，当有效范围内存在多张标签的时候，无法知道这个命令操作的是哪一张电子标签。
        /// </summary>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。待设定读保护的电子标签访问密码必须不为0，访问密码为0 的电子标签是无法设置读保护的，在命令中，必须给出正确的访问密码。</param>
        /// <returns></returns>
        public async Task<InfoBase> SetMultiReadProtectAsync(byte[] Pwd)
        {
            byte[] frame = CreateSetMultiReadProtectFrame(Pwd);
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
        /// 这个命令用来给设置了读保护的标签解锁。用这个命令时，天线有效范围内只能放置一张要被解锁的电子标签。仅对NXP UCODE EPC G2X标签有效。
        /// </summary>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。命令中必须给出正确的访问密码。</param>
        /// <returns></returns>
        public async Task<InfoBase> RemoveReadProtectAsync(byte[] Pwd)
        {
            byte[] frame = CreateRemoveReadProtectFrame(Pwd);
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
        /// 这个命令不能测试标签是否支持读保护锁定命令，只能测试标签是否被读保护锁定。对于不支持读保护锁定的电子标签，一致认为没有被锁定。
        /// 这个命令只能对单张电子标签进行操作，确保天线有效范围内只存在一张电子标签。仅对NXP的UCODE EPC G2X标签有效。
        /// </summary>
        /// <returns></returns>
        public async Task<CheckReadProtectedInfo> CheckReadProtectedAsync()
        {
            byte[] frame = CreateCheckReadProtectedFrame();
            CommunicationReturnInfo cri = await com.SendAsync(frame);
            if (cri.ReturnValue != ReturnMessage.Success)
            {
                CheckReadProtectedInfo crpi = new CheckReadProtectedInfo();
                crpi.SendByte = frame;
                crpi.ReturnValue = cri.ReturnValue;
                crpi.ExceptionMessage = cri.ExceptionMessage;
                return crpi;
            }

            CheckReadProtectedInfo info = HandelCheckReadProtectedFrame(cri.RecvByte);
            info.SendByte = frame;
            return info;
        }

        /// <summary>
        /// 对电子标签的EAS状态位进行设置或复位。仅对NXP UCODE EPC G2标签有效。
        /// </summary>
        /// <param name="ENum">EPC号长度。以字为单位。EPC的长度在15个字以内，可以为0，否则返回参数错误信息。</param>
        /// <param name="EPC">要写入数据的标签的EPC号。长度由所给的EPC号决定，EPC号以字为单位，且必须是整数个长度。高字在前，每个字的高字节在前。这里要求给出的是完整的EPC号。</param>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。待设置的标签的访问密码必须不为0，访问密码为0的电子标签是无法设置EAS报警的。Pwd必须是正确的访问密码。</param>
        /// <param name="EAS">1个字节。Bit0位为0，表示设置为关闭EAS报警；为1，表示设置为打开EAS报警。Bit1 – Bit7 位保留，默认为0。</param>
        /// <param name="MaskAdr">一个字节，掩模EPC号的起始字节地址。0x00表示从EPC号的最高字节开始掩模，0x01表示从EPC号的第二字节开始掩模，以此类推。</param>
        /// <param name="MaskLen">一个字节，掩模的字节数。掩模起始字节地址+掩模字节数不能大于EPC号字节长度，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> SetEASAlarmAsync(byte ENum, byte[] EPC,
            byte[] Pwd, byte EAS, byte MaskAdr, byte MaskLen)
        {
            byte[] frame = CreateSetEASAlarmFrame(ENum, EPC, Pwd, EAS, MaskAdr, MaskLen);
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
        /// 该命令检测电子标签的EAS报警。仅对NXP UCODE EPC G2标签有效。
        /// 无EAS报警的时候，返回“无电子标签可操作”消息。
        /// </summary>
        /// <returns></returns>
        public async Task<InfoBase> CheckEASAlarmAsync()
        {
            byte[] frame = CreateCheckEASAlarmFrame();
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
        /// 这个命令每次永久锁定user区中的32bits数据，锁定后，这32bits数据只能读，不能被再次写，也不能被擦除。这个命令仅对NXP UCODE EPC G2电子标签有效。
        /// </summary>
        /// <param name="ENum">EPC号长度。以字为单位。EPC的长度在15个字以内，可以为0，否则返回参数错误信息。</param>
        /// <param name="EPC">要写入数据的标签的EPC号。长度由所给的EPC号决定，EPC号以字为单位，且必须是整数个长度。高字在前，每个字的高字节在前。这里要求给出的是完整的EPC号。</param>
        /// <param name="Pwd">4个字节的访问密码。32位的访问密码的最高位在Pwd的第一字节(从左往右)的最高位，访问密码最低位在Pwd第四字节的最低位，Pwd的前两个字节放置访问密码的高字。User区块锁操作时必须给出正确的访问密码。</param>
        /// <param name="WrdPointer">要锁定的字地址。一次会锁定2个字</param>
        /// <param name="MaskAdr">一个字节，掩模EPC号的起始字节地址。0x00表示从EPC号的最高字节开始掩模，0x01表示从EPC号的第二字节开始掩模，以此类推</param>
        /// <param name="MaskLen">一个字节，掩模的字节数。掩模起始字节地址+掩模字节数不能大于EPC号字节长度，否则返回参数错误信息。</param>
        /// <returns></returns>
        public async Task<InfoBase> LockUserBlockAsync(byte ENum, byte[] EPC,
            byte[] Pwd, byte WrdPointer, byte MaskAdr, byte MaskLen)
        {
            byte[] frame = CreateLockUserBlockFrame(ENum, EPC, Pwd, WrdPointer, MaskAdr, MaskLen);
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
