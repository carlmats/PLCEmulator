using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device
{
    public abstract class IODevice : DeviceBase
    {

        public IODevice()
        {
            DataMapIn = new Dictionary<Enum, Datablock>();
            DataMapOut = new Dictionary<Enum, Datablock>();
        }

        public virtual void ToggleActivate()
        {
            Active = !Active;
        }

        public abstract void OnActiveChanged(bool newStatus);
        
        [Browsable(false)]
        public Dictionary<Enum, Datablock> DataMapIn { get; protected set; }

        [Browsable(false)]
        public Dictionary<Enum, Datablock> DataMapOut { get; protected set; }

        [Browsable(false)]
        public virtual void InputReceived() { }


        [Category("Action")]
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                OnPropertyChanged("Active");
                OnActiveChanged(value);
            }
        }

        protected bool _active;

    }
}
