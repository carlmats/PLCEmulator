using GraphX.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLCEmulator.Model.Graph
{
    public class VertexControlPE : VertexControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
           DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(VertexControlPE));


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        
        public VertexControlPE(object vertexData, bool tracePositionChange = true, bool bindToDataObject = true) : base(vertexData, tracePositionChange, bindToDataObject)
        {

        }
    }
}
