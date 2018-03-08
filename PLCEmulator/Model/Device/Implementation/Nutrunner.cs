using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device
{
    public class Nutrunner : IODevice
    {
        private enum DataKeyIn { KeepAliveAckA, KeepAliveAckB, ToolEnable, PsetSelect, SelControlGreenLight1, SelControlRedLight1 };

        private enum DataKeyOut { KeepAliveAckA, KeepAliveAckB, PFChannelIdA, PFChannelIdB, TighteningOK, TighteningNOK, PFReady, RunningPsetId, LiftedSocketNumberDevId, LiftedSocketNumberLftSc }


        public Nutrunner()
        {
            //Byte 1
            DataMapOut.Add(DataKeyOut.KeepAliveAckA, new Datablock(new Range(192, 192), 1));
            DataMapOut.Add(DataKeyOut.KeepAliveAckB, new Datablock(new Range(192, 192), 2));
            DataMapOut.Add(DataKeyOut.PFChannelIdA, new Datablock(new Range(192, 192), 4));
            DataMapOut.Add(DataKeyOut.PFChannelIdB, new Datablock(new Range(192, 192), 8));
            DataMapOut.Add(DataKeyOut.TighteningOK, new Datablock(new Range(192, 192), 16));
            DataMapOut.Add(DataKeyOut.TighteningNOK, new Datablock(new Range(192, 192), 32));
            DataMapOut.Add(DataKeyOut.PFReady, new Datablock(new Range(192, 192), 255));

            //Byte 2-*
            //DataMapOut.Add(DataKeyOut.RunningPsetId, new Datablock(new Range(193, 193)));
            //DataMapOut.Add(DataKeyOut.LiftedSocketNumberDevId, new Datablock(new Range(194, 194)));
            //DataMapOut.Add(DataKeyOut.LiftedSocketNumberLftSc, new Datablock(new Range(195, 195)));

            TighteningOK = true;

        }

        [Category("Configuration")]
        public bool TighteningOK
        {
            get => _tighteningOK;
            set
            {
                _tighteningOK = value;
                DataMapOut[DataKeyOut.TighteningOK].BytePost = value;
                DataMapOut[DataKeyOut.TighteningNOK].BytePost = !value;
            }
        }

        public override void InputReceived()
        {
            base.InputReceived();

            //if(DataMapIn[DataKeyIn.KeepAliveAckA].ByteValue == 255)
            //{
            //    // Do stuff example
            //}
        }

        public override void OnActiveChanged(bool newStatus)
        {
           
        }

        private bool _tighteningOK;
    }
}
