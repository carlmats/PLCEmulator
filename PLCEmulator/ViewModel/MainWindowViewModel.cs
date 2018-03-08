using PLCEmulator.Debug;
using PLCEmulator.Model.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PLCEmulator.ViewModel
{
    public class MainWindowViewModel
    {
        public class DeviceDummy
        {
            public Type DeviceType { get; set; }

            public String Name { get; set; }

            public Geometry Geometry { get; set; }

        }

        public ObservableCollection<DeviceDummy> DeviceCollection { get; private set; }


        public MainWindowViewModel()
        {
            DeviceCollection = new ObservableCollection<DeviceDummy>();

            DeviceCollection.Add(new DeviceDummy { DeviceType = typeof(PLC), Name = "PLC", Geometry = (Geometry)Application.Current.Resources["PLC_icon"] });
            DeviceCollection.Add(new DeviceDummy { DeviceType = typeof(DIOAD), Name = "Turck", Geometry = (Geometry)Application.Current.Resources["DIOAD_icon"] });
            DeviceCollection.Add(new DeviceDummy { DeviceType = typeof(PBL), Name = "PBL", Geometry = (Geometry)Application.Current.Resources["PBL_icon"] });
            DeviceCollection.Add(new DeviceDummy { DeviceType = typeof(Nutrunner), Name = "Nutrunner", Geometry = (Geometry)Application.Current.Resources["Nutrunner_icon"] });
        }
    }
}
