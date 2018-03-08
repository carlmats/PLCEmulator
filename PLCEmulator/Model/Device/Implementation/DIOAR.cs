using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device.Implementation
{
    // Digital IO area rack
    public class DIOAR : DeviceHub
    {
        [Browsable(false)]
        public new bool Active
        {
            get => _active;
            set => _active = value;
        }

        public override void OnActiveChanged(bool newStatus)
        {
            throw new NotImplementedException();
        }
    }
}
