﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJYZNLib
{
    interface IPort
    {
        int Timeout { get; set; }
        ReturnMessage Open();
        Task<CommunicationReturnInfo> SendAsync(byte[] data);
    }
}
