using PLCEmulator.Common;
using PLCEmulator.Debug;
using PLCEmulator.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PLCEmulator.Model.Device
{
    public class PLC : DeviceHub
    {
        public PLC()
        {
            _datablockToDevice = new byte[BLOCK_SIZE];
            _datablockToVcom = new byte[BLOCK_SIZE];
         
            Address = "192.168.56.1";
            Port = 2000;

            CycleTimeMs = 60;

        }

        private void Server_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.Length > 0)
            {
                lock (_lockDevice)
                    _datablockToDevice = e.Data;
                lock (_lockVcom)
                    HeaderHelper.UpdateHeaderOut(ref _datablockToVcom);

                if(HeaderHelper.ParseHeaderIn(e.Data).requestUpdate == 1 && _server.IsConnected())
                    Task.Run(() => _server.SendData(_datablockToVcom));
            }
        }


        private async void ReadWriteDevice()
        {
            while (!_wtoken.IsCancellationRequested)
            {
                // Clear the datablocks in case a device got removed
                if (_server != null && _server.IsConnected() && DeviceSlots.Count > 0)
                {
                    lock (_lockVcom)
                        ReadDevices(ref _datablockToVcom);
                    lock (_lockDevice)
                        WriteDevices(ref _datablockToDevice);
                }

                await Task.Delay(CycleTimeMs);
            }
        }

        // TODO: Improve implementation of threading & tasks
        public override async void OnActiveChanged(bool newStatus)
        {
            if(!newStatus)
            {
                _wtoken.Cancel();
                _server?.Dispose();
            }
            else
            {
                Loading = true;
                StartupError = false;

                _wtoken = new CancellationTokenSource();
                await Task.Factory.StartNew(ReadWriteDevice, _wtoken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                // VDCOM connection
                _server = new Server();
                _server.DataReceived += Server_DataReceived;
                 bool started = await _server.Start(_address, _port, BLOCK_SIZE);

                if(!started)
                {
                    if(Active) Active = false;
                    StartupError = true;
                    Logger.Instance.WriteLog("Failed to establish a connection with PLC");
                }

                Loading = false;
            }
        }

        [Category("Network")]
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }

        [Category("Network")]
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }

        [Category("Configuration"), DisplayName("Cycle time (ms)")]
        public int CycleTimeMs
        {
            get => _cycleTimeMs;
            set
            {
                _cycleTimeMs = value;
                OnPropertyChanged("CycleTimeMs");
            }
        }

        [Browsable(false)]
        public bool Loading
        {
            get => _loading;
            set
            {
                _loading = value;
                OnPropertyChanged("Loading");
            }
        }

        [Browsable(false)]
        public bool StartupError
        {
            get => _startupError;
            set
            {
                _startupError = value;
                OnPropertyChanged("StartupError");
            }
        }

        private int _cycleTimeMs;
        private bool _loading;
        private bool _startupError;
        private string _address;
        private int _port;
        private byte[] _datablockToDevice;
        private byte[] _datablockToVcom;
        private const int BLOCK_SIZE = 8192;
        private Object _lockDevice = new Object();
        private Object _lockVcom = new Object();
        private CancellationTokenSource _wtoken;
        private Server _server;
    }
}
