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

        protected DataBlock BinaryDeviceIn { get; set; }

        protected DataBlock BinaryDeviceOut { get; set; }


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
                    // TODO: Must calculate start range for multiple devices of same type.
                    // Use offset to shift the data in the datablock for multiple nutrunners etc.
                    foreach (var datablockMap in iodevice.DataMapOut)
                    {
                        using (var dataEntryEnumerator = datablockMap.Value.GetDataEntries())
                        {
                            while (dataEntryEnumerator.MoveNext())
                            {
                                var curData = dataEntryEnumerator.Current;
                                if (iodevice.Active && datablockMap.Value.PostByte)
                                {
                                    if (datablockMap.Value.IsByteOwner)
                                        datablock[curData.Key] = curData.Value;
                                    else
                                        datablock[curData.Key] |= curData.Value;
                                }
                                else
                                {
                                    if (datablockMap.Value.IsByteOwner)
                                        datablock[curData.Key] = 0;
                                    else
                                        datablock[curData.Key] &= (byte)~curData.Value;
                                }
                            }
                        }
                    }
                }
                else if ((device is BinaryDevice))
                {

                    if ((device as BinaryDevice).Triggered)
                    {
                        datablock[BinaryDeviceOut.GetRange().Start] |= (byte)(1 << slot);
                    }
                    else
                    {
                        datablock[BinaryDeviceOut.GetRange().Start] &= (byte)~(1 << slot);
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

                var iodevice = device as IODevice;
                if (iodevice != null)
                {
                    // TODO: Must calculate start range for multiple devices of same type.
                    // Use offset to shift the data in the datablock for multiple nutrunners etc.
                    foreach (var datablockMap in iodevice.DataMapIn)
                    {
                        using (var dataEntryEnumerator = datablockMap.Value.GetDataEntries())
                        {
                            while (dataEntryEnumerator.MoveNext())
                            {
                                var curData = dataEntryEnumerator.Current;
                                datablockMap.Value.UpdateData(curData.Key, datablock[curData.Key]);
                            }
                        }
                    }
                }
                else if (device is BinaryDevice)
                {
                    var bdev = device as BinaryDevice;
                    if ((datablock[BinaryDeviceIn.GetRange().Start] & (byte)(1 << slot)) > 0)
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

