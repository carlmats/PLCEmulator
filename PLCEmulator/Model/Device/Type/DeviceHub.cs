using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PLCEmulator.Model.Device
{
    public abstract class DeviceHub : IODevice
    {
        // Create new class that extends with event when collection changes,
        // redraw edges with new values
        [Browsable(false)]
        public ObservableConcurrentDictionary<int, IDevice> DeviceSlots { get; private set; }

        protected Datablock BinaryDeviceIn { get; set; }

        protected Datablock BinaryDeviceOut { get; set; }


        public DeviceHub()
        {
            DeviceSlots = new ObservableConcurrentDictionary<int, IDevice>();

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

                var iodevice = device as IODevice;
                if (iodevice != null)
                {
                    // TODO: Must calculate start range for multiple devices of same type
                    foreach (var map in iodevice.DataMapOut)
                    {
                        if (iodevice.Active && map.Value.BytePost)
                        {
                            datablock[map.Value.ByteIndex.Start] |= map.Value.ByteValue;
                        }
                        else
                        {
                            datablock[map.Value.ByteIndex.Start] &= (byte)~map.Value.ByteValue;
                        }
                    }
                }
                else if ((device is BinaryDevice))
                {
                    if((device as BinaryDevice).Triggered)
                    {
                        datablock[BinaryDeviceOut.ByteIndex.Start] |= (byte)(1 << slot);
                    }
                    else
                    {
                        datablock[BinaryDeviceOut.ByteIndex.Start] &= (byte)~(1 << slot);
                    }
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
                    var iodev = device as IODevice;
                    foreach (var map in iodev.DataMapIn)
                    {
                        map.Value.ByteValue = datablock[map.Value.ByteIndex.Start];
                    }

                    iodev.InputReceived();
                }
                else if (device is BinaryDevice)
                {
                    var bdev = device as BinaryDevice;
                    if ((datablock[BinaryDeviceIn.ByteIndex.Start] & (byte)(1 << slot)) > 0)
                    {
                        bdev.Active = true;
                    }
                    else
                    {
                        bdev.Active = false;
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
