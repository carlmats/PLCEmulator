using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model
{
    public interface IIODevice : IDevice
    {
        Dictionary<Enum, Datablock> DataMapIn { get; }

        Dictionary<Enum, Datablock> DataMapOut { get; }
    }
}
