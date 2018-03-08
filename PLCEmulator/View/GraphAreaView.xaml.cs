using GraphX.Controls;
using GraphX.Controls.Animations;
using GraphX.Controls.Models;
using PLCEmulator.Model.Device;
using PLCEmulator.Model.Graph;
using PLCEmulator.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PLCEmulator.View
{

    public partial class GraphAreaView : UserControl
    {
        public GraphAreaView()
        {
            InitializeComponent();
            g_area.MouseOverAnimation = AnimationFactory.CreateMouseOverAnimation(MouseOverAnimation.Scale, .3);
            g_area.DeleteAnimation = AnimationFactory.CreateDeleteAnimation(DeleteAnimation.Shrink, .5);
            g_area.ShowAllEdgesLabels(true);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            var deviceType = e.Data.GetData("DeviceType") as Type;
            if (deviceType != null)
            {
                if (DataContext is GraphAreaViewModel)
                {
                    var vm = DataContext as GraphAreaViewModel;
                    DeviceBase instance = (DeviceBase)Activator.CreateInstance(deviceType);

                    if (vm.TryAddDevice(instance))
                    {
                        g_area.AddVertex(instance, new VertexControlPE(instance));                        
                        g_area.VertexList[instance].SetPosition(e.GetPosition(g_area));
                    }
                }

                if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.Move;
                }
            }
            e.Handled = true;
        }

        // Temporary vertex control
        // TODO: Rework 
        private void g_area_VertexClicked(object sender, VertexClickedEventArgs args)
        {
            var vm = DataContext as GraphAreaViewModel;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && vm.SelectedDevice != null)
            {
                if (vm.TryAddEdge(vm.SelectedDevice, (DataVertex)args.Control.Vertex))
                {
                    g_area.GenerateAllEdges();
                }
            }
            else
            {
                var vcpe = args.Control as VertexControlPE;
                var vert = args.Control.Vertex as DeviceBase;
                if (vcpe != null && vert != null)
                {
                    DeselectVertices();
                    vcpe.IsSelected = true;
                    vm.SelectedDevice = vert;
                }
            }

        }

        private void g_zoomctrl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeselectVertices();
        }

        private void DeselectVertices()
        {
            var vm = DataContext as GraphAreaViewModel;
            vm.SelectedDevice = null;

            foreach (var vc in g_area.VertexList.Values)
            {
                var vcpe = vc as VertexControlPE;
                if (vcpe != null)
                {
                    vcpe.IsSelected = false;
                }
            }
        }

        private void EdgeText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                int newSlot;
                var ec = GetParent<EdgeControl>((Visual)e.Source);
                var vm = DataContext as GraphAreaViewModel;

                if (ec != default(EdgeControl) && vm != null && Int32.TryParse(textBox.Text, out newSlot))
                {
                    vm.TryChangeDeviceSlot(ec, newSlot);
                    g_area.UpdateAllEdges();
                }
            }


        }

        private T GetParent<T>(Visual v)
        {
            while (v != null)
            {
                v = VisualTreeHelper.GetParent(v) as Visual;
                if (v is T)
                    break;
            }

            try
            {
                return (T)Convert.ChangeType(v, typeof(T));
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }
    }
}
