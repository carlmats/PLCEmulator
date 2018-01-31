using PLCEmulator.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model
{
    // Digital IO area device
    public class DIOAD : IIODevice
    {
        private enum DataKeyIn { DeviceOnOff, Interface, Error };

        private enum DataKeyOut {  };


        public DIOAD()
        {

            DataMapIn.Add(DataKeyIn.DeviceOnOff, new Datablock(new Range(53,58))); // not 58
            DataMapIn[DataKeyIn.DeviceOnOff].ByteValue = 255;

        }
    }
}
