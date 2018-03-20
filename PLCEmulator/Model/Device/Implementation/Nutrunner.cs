using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace PLCEmulator.Model.Device
{
    [CategoryOrder("Action", 1)]
    [CategoryOrder("Tightening", 2)]
    [CategoryOrder("Status messages", 3)]
    public class Nutrunner : IODevice
    {
        private enum DataKeyIn { KeepAliveAckA, KeepAliveAckB, ToolEnable, PsetSelect, SelControlGreenLight1, SelControlRedLight1 };

        private enum DataKeyOut { KeepAliveAckA, KeepAliveAckB, PFChannelIdA, PFChannelIdB, TighteningOK, TighteningNOK, PFReady, QueueFlushed, RunningPsetId, LiftedSocketNumberDevId, LiftedSocketNumberLftSc, ErrorCode, FinalTorque, FinalAngle }


        public Nutrunner()
        {
            //Ranges & values done
            DataMapOut.TryAdd(DataKeyOut.KeepAliveAckA, new DataBlock(202, 1)); // new Data(new Range(202), 1)
            DataMapOut.TryAdd(DataKeyOut.KeepAliveAckB, new DataBlock(202, 2));
            DataMapOut.TryAdd(DataKeyOut.PFChannelIdA, new DataBlock(202, 4));
            DataMapOut.TryAdd(DataKeyOut.PFChannelIdB, new DataBlock(202, 8));
            DataMapOut.TryAdd(DataKeyOut.TighteningOK, new DataBlock(202, 16));
            DataMapOut.TryAdd(DataKeyOut.TighteningNOK, new DataBlock(202, 32));
            DataMapOut.TryAdd(DataKeyOut.PFReady, new DataBlock(202, 64));
            DataMapOut.TryAdd(DataKeyOut.QueueFlushed, new DataBlock(202, 128));

            // Ranges done
            // TODO: Values
            //DataMapOut.Add(DataKeyOut.LiftedSocketNumberDevId, new Data(new Range(204), 255));
            //DataMapOut.Add(DataKeyOut.LiftedSocketNumberLftSc, new Data(new Range(205), 255));
            //DataMapOut.Add(DataKeyOut.ErrorCode, new Data(new Range(206, 207), 255)); // 206 & 207
            DataMapOut.TryAdd(DataKeyOut.FinalTorque, new DataBlock(new Range(208, 211), true, true)); // 208 - 211
            DataMapOut.TryAdd(DataKeyOut.FinalAngle, new DataBlock(new Range(212, 215), true, true)); // 212 - 215

            //Byte 2-*
            //DataMapOut.Add(DataKeyOut.RunningPsetId, new Datablock(new Range(193, 193)));
            //DataMapOut.Add(DataKeyOut.LiftedSocketNumberDevId, new Datablock(new Range(194, 194)));
            //DataMapOut.Add(DataKeyOut.LiftedSocketNumberLftSc, new Datablock(new Range(195, 195)));

            TighteningOK = true;
        }


        // ******** ACTION ********** //

        [DisplayName("Tighten")]
        public new bool Active
        {
            get => _active;
            set
            {
                _active = value;
                OnPropertyChanged("Active");
                OnActiveChanged(value);
            }
        }

        // ******** TIGHTENING ********** //

        [Category("Tightening"), DisplayName("Tightening OK"), PropertyOrder(1)]
        public bool TighteningOK
        {
            get => _tighteningOK;
            set
            {
                _tighteningOK = value;
                DataMapOut[DataKeyOut.TighteningOK].PostByte = value;
                DataMapOut[DataKeyOut.TighteningNOK].PostByte = !value;
                OnPropertyChanged("TighteningOK");
            }
        }

        [Category("Tightening"), DisplayName("Torque"), PropertyOrder(2)]
        public ushort FinalTorque
        {
            get => _finalTorque;
            set
            {
                _finalTorque = value;
                SetDataRange(DataKeyOut.FinalTorque, value, DataMapOut);
                OnPropertyChanged("FinalTorque");
            }
        }

        [Category("Tightening"), DisplayName("Angle"), PropertyOrder(3)]
        public ushort FinalAngle
        {
            get => _finalAngle;
            set
            {
                _finalAngle = value;
                SetDataRange(DataKeyOut.FinalAngle, value, DataMapOut);
                OnPropertyChanged("FinalAngle");
            }
        }


        // ******** STATUS MESSAGES ********** //

        [Category("Status messages"), DisplayName("Queue flushed")]
        public bool QueueFlushed
        {
            get => _queueFlushed;
            set
            {
                _queueFlushed = value;
                DataMapOut[DataKeyOut.QueueFlushed].PostByte = value;
                OnPropertyChanged("QueueFlushed");
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

        private bool _tighteningOK, _queueFlushed;
        private ushort _finalTorque, _finalAngle;
    }
}
