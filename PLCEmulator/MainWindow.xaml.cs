using PLCEmulator.Model.Device;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            //var PLC = new PLC();
            //var dev = new DIOAD();
            //PLC.TryAddDevice(0,dev);
            //var pbl = new PBL();
            //dev.TryAddDevice(0, pbl);
            //pbl.Active = false;
            //Thread.Sleep(20000);
            //pbl.Active = true;
        }

        private void DeviceDragList_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var listView = sender as ListView;
            if (e.LeftButton == MouseButtonState.Pressed && listView != null && listView.SelectedItem is ViewModel.MainWindowViewModel.DeviceDummy)
            {
                var deviceDummy = listView.SelectedItem as ViewModel.MainWindowViewModel.DeviceDummy;
                DataObject data = new DataObject("DeviceType", deviceDummy.DeviceType);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.ArrowCD);
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;
        }
    }
}
