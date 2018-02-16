using PLCEmulator.Common;
using PLCEmulator.Network.VDCOM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

            _header = new Header.Content();

            // Read & write connected devices
            _wtoken = new CancellationTokenSource();
            Task.Factory.StartNew(ReadWriteDevice, _wtoken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // VDCOM connection
            _server = new Server("192.168.56.1", 2000, BLOCK_SIZE);
            _server.DataReceived += Server_DataReceived;

        }

        private void Server_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.Length > 0)
            {
                lock (_lockDevice)
                    _datablockToDevice = e.Data;
                lock (_lockVcom)
                    Header.UpdateHeader(ref _header, ref _datablockToVcom);

                // TODO: Check for update flag before writing?
                Task.Run(() => _server.SendData(_datablockToVcom));
            }
        }


        private async void ReadWriteDevice()
        {
            while (!_wtoken.IsCancellationRequested)
            {
                // Clear the datablocks in case a device got removed
                if (DeviceSlots.Count > 0)
                {
                    lock (_lockVcom)
                        ReadDevices(ref _datablockToVcom);
                    lock (_lockDevice)
                        WriteDevices(ref _datablockToDevice);
                }
                // TODO: Calculate correct delay
                await Task.Delay(100);
            }
        }

        private byte[] _datablockToDevice;
        private byte[] _datablockToVcom;
        private const int BLOCK_SIZE = 8192;
        private Object _lockDevice = new Object();
        private Object _lockVcom = new Object();
        private CancellationTokenSource _wtoken;
        private Header.Content _header;
        private Server _server;
    }
}
