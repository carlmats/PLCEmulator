using PLCEmulator.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PLCEmulator.Model.Device
{
    public class DeviceHub : IODevice
    {
        public ConcurrentDictionary<int, IDevice> DeviceSlots { get; private set; }

        protected Datablock BinaryDeviceIn { get; set; }

        protected Datablock BinaryDeviceOut { get; set; }


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

                if (device is IODevice)
                {
                    foreach (var map in (device as IODevice).DataMapOut)
                    {
                        datablock[map.Value.Range.Start] = map.Value.ByteValue;
                    }
                }
                else if ((device is BinaryDevice))
                {
                    if((device as BinaryDevice).Triggered)
                        datablock[BinaryDeviceOut.Range.Start] |= (byte)(1 << slot);
                    else
                        datablock[BinaryDeviceOut.Range.Start] &= (byte)~(1 << slot);
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

                if (device is IODevice)
                {
                    foreach (var map in (device as IODevice).DataMapIn)
                    {
                        map.Value.ByteValue = datablock[map.Value.Range.Start];
                    }
                }
                else if (device is BinaryDevice)
                {
                    if ((datablock[BinaryDeviceIn.Range.Start] & (byte)(1 << slot)) > 0)
                    {
                        (device as BinaryDevice).Active = true;
                    }
                    else
                    {
                        (device as BinaryDevice).Active = false;
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
