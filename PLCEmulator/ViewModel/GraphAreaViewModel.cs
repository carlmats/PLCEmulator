using PLCEmulator.Model.Device;
using PLCEmulator.Model.Graph;

namespace PLCEmulator.ViewModel
{
    public class GraphAreaViewModel
    {

        public GXLogicCorePE LogicCore { get; set; }

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
                dhtarget.TryAddDevice(dhtarget.DeviceSlots.Count, dsource);
                return LogicCore.Graph.AddEdge(new DataEdge(source, target));
            }
            else return false;
        }
    }
}
