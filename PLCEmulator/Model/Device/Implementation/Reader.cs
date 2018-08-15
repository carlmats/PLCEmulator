using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device
{
    public class Reader : IODevice
    {
        // Dataman8500 = 1,2=id, 5,6=len, 4=readOK, 7->len=streckod
        private enum DataKeyIn { tmp1, tmp2 };

        private enum DataKeyOut { DeviceOnOff,tmp1, tmp2, tmp3, tmp4 }


        public Reader()
        {
            DataMapOut.TryAdd(DataKeyOut.DeviceOnOff, new DataBlock(65, 4, false, true));

            // ReadWriteAreaStart+k+(i*128) (352+bit+(rwIndex*128))
            int index1 = 352 + 0 + (1 * 128) + 10;
            DataMapOut.TryAdd(DataKeyOut.tmp1, new DataBlock(new Range(index1, index1 + 1), BitConverter.GetBytes(_id), true, true)); // id
            DataMapOut.TryAdd(DataKeyOut.tmp2, new DataBlock(index1+3, 255, true, true)); // Read ok


            var test = BitConverter.GetBytes(11*2);

            var test2 = new byte[] { test[1], test[0] };
            Encoding enc = Encoding.GetEncoding(28591);

            DataMapOut.TryAdd(DataKeyOut.tmp3, new DataBlock(new Range(index1 + 4, index1 + 5), test2, true, true)); // Len
            DataMapOut.TryAdd(DataKeyOut.tmp4, new DataBlock(new Range(index1 + 6, index1 + 6 + enc.GetByteCount("P12324#TVEC")-1), enc.GetBytes("P12324TVEC#"), true, true));

            DataMapOut.TryAdd(DataKeyOut.tmp1, new DataBlock(202, 1)); // 208 - 211
            DataMapOut.TryAdd(DataKeyOut.tmp2, new DataBlock(202, 1)); // 212 - 215

            BitConverter.GetBytes(11);



        }


        public override void OnActiveChanged(bool newStatus)
        {
            _id++;
            DataMapOut[DataKeyOut.tmp1] = new DataBlock(new Range(352 + 0 + (1 * 128) + 10, 352 + 0 + (1 * 128) + 10 + 1), BitConverter.GetBytes(_id), true, true);
        }


        private int _id = 1;
    }
}
