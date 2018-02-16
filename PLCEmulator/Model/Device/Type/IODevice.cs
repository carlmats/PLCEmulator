using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device
{
    public abstract class IODevice : DeviceBase
    {
        public Dictionary<Enum, Datablock> DataMapIn { get; protected set; }

        public Dictionary<Enum, Datablock> DataMapOut { get; protected set; }
    }
}
