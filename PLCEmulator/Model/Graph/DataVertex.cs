using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Models;
using System.ComponentModel;

namespace PLCEmulator.Model.Graph
{
    //Vertex data object
    public class DataVertex : VertexBase
    {
        [Browsable(false)]
        public new double Angle { get; set; }

        [Browsable(false)]
        public new int GroupId { get; set; }

        [Browsable(false)]
        public new ProcessingOptionEnum SkipProcessing { get; set; }

        [Browsable(false)]
        public new long ID { get; set; }

    }
}
