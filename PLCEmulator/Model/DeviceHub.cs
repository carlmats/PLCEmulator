using PLCEmulator.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model
{
    public class DeviceHub : IIODevice
    {
        protected ConcurrentDictionary<int, IDevice> DeviceSlots { get; private set; }

        protected Datablock BinaryDeviceIn { get; set; }

        protected Datablock BinaryDeviceOut { get; set; }

        public Dictionary<Enum, Datablock> DataMapIn { get; private set; }

        public Dictionary<Enum, Datablock> DataMapOut { get; private set; }


        public DeviceHub()
        {
            DeviceSlots = new ConcurrentDictionary<int, IDevice>();
            DataMapIn = new Dictionary<Enum, Datablock>();
            DataMapOut = new Dictionary<Enum, Datablock>();
        }

        public bool TryAddDevice(int index, IDevice device)
        {
             return DeviceSlots.TryAdd(index, device);
        }

        public bool TryRemoveDevice(IDevice device)
        {
            return DeviceSlots.Values.Remove(device);
        }

        public int DeviceCount() => DeviceSlots.Count;

        protected void ReadDevices(ref byte[] datablock)
        {
            foreach (var deviceSlot in DeviceSlots)
            {
                int slot = deviceSlot.Key;
                IDevice device = deviceSlot.Value;

                if (device is IIODevice)
                {
                    foreach (var map in (device as IIODevice).DataMapOut)
                    {
                        datablock[map.Value.Range.Start] = map.Value.ByteValue;
                    }
                }
                else if ((device is IBinaryDevice) && (device as IBinaryDevice).Active)
                {
                    datablock[BinaryDeviceOut.Range.Start] |= (byte)(1 << slot);
                }

                // Connected DeviceHub
                if (device is DeviceHub)
                {
                    (device as DeviceHub).ReadDevices(ref datablock);
                }
            }
        }

        protected void WriteDevices(ref byte[] datablock)
        {
            foreach (var deviceSlot in DeviceSlots)
            {
                int slot = deviceSlot.Key;
                IDevice device = deviceSlot.Value;

                if (device is IIODevice)
                {
                    foreach (var map in (device as IIODevice).DataMapIn)
                    {
                        map.Value.ByteValue = datablock[map.Value.Range.Start];
                    }
                }
                else if (device is IBinaryDevice)
                {
                    if((datablock[BinaryDeviceIn.Range.Start] & (byte)(1 << slot)) > 0)
                    {
                        (device as IBinaryDevice).Active = true;
                    }
                }
                if (device is DeviceHub)
                {
                    (device as DeviceHub).WriteDevices(ref datablock);
                }
            }
        }


    }
}
