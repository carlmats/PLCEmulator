using GraphX.Controls;
using PLCEmulator.Common;
using PLCEmulator.Model.Device;
using PLCEmulator.Model.Graph;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace PLCEmulator.ViewModel
{
    public class GraphAreaViewModel : INotifyPropertyChanged
    {
        private DataVertex _selectedDevice;

        private ICommand _toggleActivateCommand;
        private ICommand _toggleTriggeredCommand;


        public ICommand ToggleActivateCommand
        {
            get
            {
                if (_toggleActivateCommand == null)
                {
                    _toggleActivateCommand = new RelayCommand(
                        param => ToggleActivate(),
                        param => CanToggleActivate()
                    );
                }
                return _toggleActivateCommand;
            }
        }

        public ICommand ToggleTriggeredCommand
        {
            get
            {
                if (_toggleTriggeredCommand == null)
                {
                    _toggleTriggeredCommand = new RelayCommand(
                        param => ToggleTriggered(),
                        param => CanToggleTriggered()
                    );
                }
                return _toggleTriggeredCommand;
            }
        }


        private bool CanToggleActivate()
        {
            return _selectedDevice is IODevice;
        }

        private void ToggleActivate()
        {
            (_selectedDevice as IODevice).ToggleActivate();
        }


        private bool CanToggleTriggered()
        {
            return _selectedDevice is BinaryDevice;
        }

        private void ToggleTriggered()
        {
            (_selectedDevice as BinaryDevice).ToggleTriggered();
        }

        public GXLogicCorePE LogicCore { get; set; }

        public DataVertex SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public GraphAreaViewModel()
        {
            LogicCore = new GXLogicCorePE();
            LogicCore.Graph = new GraphPE();
        }

        public bool TryAddDevice(DeviceBase device)
        {
            return LogicCore.Graph.AddVertex(device);
        }

        public bool TryAddEdge(DataVertex source, DataVertex target)
        {
            var dhtarget = target as DeviceHub;
            var dsource = source as IDevice;

            if (dhtarget != null && dsource != null && !dhtarget.DeviceSlots.Values.Contains(dsource))
            {
                int slot = 0;
                while(!dhtarget.TryAddDevice(slot, dsource) && slot < 100) slot++;

                return LogicCore.Graph.AddEdge(new DataEdge(source, target) { Text = dhtarget.DeviceSlots.FirstOrDefault(x => x.Value == dsource).Key.ToString()});
            }
            else return false;
        }

        public bool TryChangeDeviceSlot(EdgeControl ec, int newSlot)
        {
            var dh = ec.Target.Vertex as DeviceHub;
            var de = ec.Edge as DataEdge;
            if (dh != null && de != null)
            {
                int oldSlot;
                try
                {
                    oldSlot = dh.DeviceSlots.First(x => x.Value == (ec.Source.Vertex)).Key;
                }
                catch
                {
                    return false;
                }

                IDevice device;
                bool result = dh.DeviceSlots.TryRemove(oldSlot, out device);
                result &= dh.DeviceSlots.TryAdd(newSlot, device);
                if (result) de.Text = newSlot.ToString();

                return result;
            }
            return false;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
