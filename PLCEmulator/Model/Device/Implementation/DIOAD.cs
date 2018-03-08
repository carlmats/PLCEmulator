using PLCEmulator.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device
{
    // Digital IO area device
    public class DIOAD : DeviceHub
    {
        private enum DataKeyIn { SimpleDevice };

        private enum DataKeyOut { DeviceOnOff, SimpleDevice, Error };


        public DIOAD()
        {
            DataMapOut.Add(DataKeyOut.DeviceOnOff, new Datablock(new Range(53,58), 255));

            BinaryDeviceOut = new Datablock(new Range(170, 195), 0); // 170 works why ??? not 195
            BinaryDeviceIn = new Datablock(new Range(72, 93), 0); // not 93

            Active = true;

        }

        //[Browsable(false)]
        //public new bool Active
        //{
        //    get =>  _active;
        //    set => _active = value;
        //}

        public override void OnActiveChanged(bool newStatus)
        {
            if (!newStatus) DataMapOut[DataKeyOut.DeviceOnOff].BytePost = false;
            else DataMapOut[DataKeyOut.DeviceOnOff].BytePost = true;
        }
    }
}
