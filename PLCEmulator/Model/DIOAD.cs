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
    public class DIOAD : DeviceHub
    {
        private enum DataKeyIn { SimpleDevice };

        private enum DataKeyOut { DeviceOnOff, SimpleDevice, Error };


        public DIOAD()
        {
            DataMapOut.Add(DataKeyOut.DeviceOnOff, new Datablock(new Range(53,58))); // not 58
            DataMapOut[DataKeyOut.DeviceOnOff].ByteValue = 255;

            BinaryDeviceOut = new Datablock(new Range(170, 195)); // 170 works why ??? not 195
            BinaryDeviceIn = new Datablock(new Range(63, 93)); // not 93


        }
    }
}
