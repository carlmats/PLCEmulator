using PLCEmulator.Model;
using System.Windows;

namespace PLCEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var PLC = new PLC();
            var dev = new DIOAD();
            PLC.TryAddDevice(0,dev);
            var pbl = new PBL();
            dev.TryAddDevice(0, pbl);
            //pbl.Active = false;
            //Thread.Sleep(20000);
            //pbl.Active = true;
        }
    }
}
