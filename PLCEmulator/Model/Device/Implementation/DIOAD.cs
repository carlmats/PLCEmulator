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
        private enum DataKeyIn { Active };

        private enum DataKeyOut { DeviceOnOff, SimpleDevice, Error };


        public DIOAD()
        {
            // OUTPUT
            DataMapOut.TryAdd(DataKeyOut.DeviceOnOff, new DataBlock(53, 255)); // new Range(53,58)


            // INPUT
            var ActiveBlock = new DataBlock(15, true);
            if(DataMapIn.TryAdd(DataKeyIn.Active, ActiveBlock))
            {
                ActiveBlock.DataChanged += ActiveBlock_DataChanged;
            }

            // BINARY DEVICE
            BinaryDeviceOut = new DataBlock(new Range(170, 195)); // 170 works why ??? not 195
            BinaryDeviceIn = new DataBlock(new Range(72, 93)); // not 93


        }

        private void ActiveBlock_DataChanged(DataBlock dataBlock, DataBlock.DataChangedEventArgs e)
        {
            if (e.NewData == 1) Active = true;
            else Active = false;
        }

        public override void OnActiveChanged(bool newStatus)
        {
            if (!newStatus) DataMapOut[DataKeyOut.DeviceOnOff].PostByte = false;
            else DataMapOut[DataKeyOut.DeviceOnOff].PostByte = true;
        }
    }
}
