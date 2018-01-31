using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model
{
    // Digital IO area device
    public class DIOAD : IDevice, IIODevice
    {
        private enum DataKeyIn { DeviceOnOff, Interface, Error };

        private enum DataKeyOut {  };

        public Dictionary<Enum, Datablock> DataMapIn { get; protected set; }

        public Dictionary<Enum, Datablock> DataMapOut { get; protected set; }

        public List<IDevice> Devices { get; protected set; }

        public DIOAD()
        {
            DataMapIn = new Dictionary<Enum, Datablock>();
            DataMapIn.Add(DataKeyIn.DeviceOnOff, new Datablock(new Range(53,58))); // not 58
            DataMapIn[DataKeyIn.DeviceOnOff].ByteValue = 255;

            DataMapOut = new Dictionary<Enum, Datablock>();
        }
    }
}
