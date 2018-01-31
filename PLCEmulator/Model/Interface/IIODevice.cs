using PLCEmulator.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model
{
    public class IIODevice : IDevice
    {
        // Due to lack of a thread safe HashSet we use dictionary
        public ConcurrentDictionary<IDevice, byte> Devices { get; }

        public Dictionary<Enum, Datablock> DataMapIn { get; protected set; }

        public Dictionary<Enum, Datablock> DataMapOut { get; protected set; }

        public IIODevice()
        {
            Devices = new ConcurrentDictionary<IDevice, byte>();

            DataMapIn = new Dictionary<Enum, Datablock>();
            DataMapOut = new Dictionary<Enum, Datablock>();
        }

        public bool TryAddDevice(IDevice device)
        {
            return Devices.TryAdd(device, 0);
        }

        public bool TryRemoveDevice(IDevice device)
        {
            byte trash;
            return Devices.TryRemove(device, out trash);
        }

    }
}
