using MahApps.Metro.Controls;
using PLCEmulator.Debug;
using PLCEmulator.Model.Device;
using PLCEmulator.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PLCEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private GraphAreaViewModel _graphAreaViewModel { get; set; }
        public MainWindowViewModel _mainWindowViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _graphAreaViewModel = new GraphAreaViewModel();
            _mainWindowViewModel = new MainWindowViewModel();

            this.DataContext = _mainWindowViewModel;
            _graphAreaView.DataContext = _graphAreaViewModel;
            _propertyGrid.DataContext = _graphAreaViewModel;


            Logger.Instance.Log.CollectionChanged += Log_CollectionChanged;
        }

        private void Log_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LogView.SelectedIndex = Logger.Instance.Log.Count - 1;

            if (VisualTreeHelper.GetChildrenCount(LogView) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(LogView, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }

        }

        private void DeviceDragList_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var listView = sender as ListView;
            if (e.LeftButton == MouseButtonState.Pressed && listView != null && listView.SelectedItem is MainWindowViewModel.DeviceDummy)
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
