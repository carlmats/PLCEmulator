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
        private VertexControl _markedVC = null;

        public GraphAreaView()
        {
            InitializeComponent();
            g_area.MouseOverAnimation = AnimationFactory.CreateMouseOverAnimation(MouseOverAnimation.Scale, .3);
            g_area.DeleteAnimation = AnimationFactory.CreateDeleteAnimation(DeleteAnimation.Shrink, .5);

            var vm = DataContext as GraphAreaViewModel;
            g_area.GenerateGraph();
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
                        g_area.AddVertex(instance, new VertexControl(instance));                        
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
            if(Keyboard.IsKeyDown(Key.A))
            {
                (args.Control.Vertex as PBL).Triggered = !(args.Control.Vertex as PBL).Triggered;
            }

            if(Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if(_markedVC == null)
                {
                    _markedVC = args.Control;
                    _markedVC.BorderBrush = Brushes.BlueViolet;
                }
                else if(_markedVC != args.Control)
                {
                    var vm = DataContext as GraphAreaViewModel;

                    if (vm.TryAddEdge((DataVertex)_markedVC.Vertex, (DataVertex)args.Control.Vertex))
                    {
                        _markedVC.BorderBrush = Brushes.Black;
                        g_area.GenerateAllEdges();
                        _markedVC = null;
                    }
                }

            }
            else
            {
                if(_markedVC != null)
                {
                    _markedVC.BorderBrush = Brushes.Black;
                    _markedVC = null;
                }

            }
        }

        private void g_zoomctrl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_markedVC != null)
            {
                _markedVC.BorderBrush = Brushes.Black;
                _markedVC = null;
            }
        }
    }
}
