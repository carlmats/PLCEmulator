using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLCEmulator.Common;

namespace PLCEmulator.Model
{
    public class PBL : IDevice
    {
        public Dictionary<Enum, Datablock> DataMapIn { get; set; }

        public Dictionary<Enum, Datablock> DataMapOut { get; set; }
    }
}
