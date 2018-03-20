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
            DataMapIn = new ObservableConcurrentDictionary<Enum, DataBlock>();
            DataMapOut = new ObservableConcurrentDictionary<Enum, DataBlock>();
        }

        public virtual void ToggleActivate()
        {
            Active = !Active;
        }

        public void SetDataRange(Enum key, uint value, ObservableConcurrentDictionary<Enum, DataBlock> dataMap)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            using (var dataEntryEnumarator = dataMap[key].GetDataEntries())
            {
                int i = 0;
                while (dataEntryEnumarator.MoveNext() && i < bytes.Length)
                {
                    dataMap[key].UpdateData(dataEntryEnumarator.Current.Key, bytes[i++]);
                }
            }
        }

        public abstract void OnActiveChanged(bool newStatus);
        
        [Browsable(false)]
        public ObservableConcurrentDictionary<Enum, DataBlock> DataMapIn { get; protected set; }

        [Browsable(false)]
        public ObservableConcurrentDictionary<Enum, DataBlock> DataMapOut { get; protected set; }

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
