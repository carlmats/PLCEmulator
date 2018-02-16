using GraphX.PCL.Common.Models;

namespace PLCEmulator.Model.Graph
{
    //Vertex data object
    public class DataVertex : VertexBase
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }

    }
}
