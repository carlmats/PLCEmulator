using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device
{
    public abstract class BinaryDevice : DeviceBase
    {
        protected bool _active;
        protected bool _triggered;
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                OnPropertyChanged("Active");
            }
        }

        public bool Triggered
        {
            get => _triggered;
            set
            {
                _triggered = value;
                OnPropertyChanged("Triggered");
            }
        }
    }
}
