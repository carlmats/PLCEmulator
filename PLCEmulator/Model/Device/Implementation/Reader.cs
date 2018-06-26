using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device
{
    public class Reader : IODevice
    {
        public override void OnActiveChanged(bool newStatus)
        {
            throw new NotImplementedException();
        }
    }
}
