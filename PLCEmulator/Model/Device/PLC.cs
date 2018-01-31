using PLCEmulator.Common;
using PLCEmulator.Network.VDCOM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PLCEmulator.Model
{
    public class PLC : IIODevice
    {
        public PLC()
        {
            _datablockIn = new byte[BLOCK_SIZE];
            _datablockOut = new byte[BLOCK_SIZE];

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
                lock (_lockIn)
                {
                    _datablockIn = e.Data;
                }
            
                lock (_lockOut)
                {
                    Header.UpdateHeader(ref _header, ref _datablockOut);
                }

                // TODO: Check for update flag before writing?
                Task.Run(() => _server.SendData(_datablockOut));
            }
        }

        private void ReadDevice(IDevice device)
        {
            if (device.DataMapIn == null) return;

            foreach (var map in device.DataMapIn)
            {
                var datablock = map.Value;
                lock(_lockOut)
                {
                    _datablockOut[datablock.Range.Start] = datablock.ByteValue; // Start + index
                }
            }

            var iodevice = device as IIODevice;
            if (iodevice != null && iodevice.Devices != null)
            {
                foreach (var condevice in iodevice.Devices)
                {
                    ReadDevice(condevice.Key);
                }
            }
        }

        private void WriteDevice(IDevice device)
        {
            if (device.DataMapOut == null) return;

            foreach (var map in device.DataMapOut)
            {
                var datablock = map.Value;
                lock(_lockIn)
                {
                    datablock.ByteValue = _datablockIn[datablock.Range.Start]; // Start + index
                }
            }

            var iodevice = device as IIODevice;
            if (iodevice != null && iodevice.Devices != null)
            {
                foreach (var condevice in iodevice.Devices)
                {
                    WriteDevice(condevice.Key);
                }
            }
        }

        private async void ReadWriteDevice()
        {
            while (!_wtoken.IsCancellationRequested)
            {
                // Clear the datablocks in case a device got removed
                lock (_lockIn)
                    lock (_lockOut)
                    {
                        Array.Clear(_datablockOut, 0, _datablockOut.Length);
                        Array.Clear(_datablockIn, 0, _datablockIn.Length);
                    }

                if (Devices.Count > 0)
                {
                    ReadDevice(this);
                    WriteDevice(this);
                }

                // TODO: Calculate correct delay
                await Task.Delay(100);
            }
        }

        private byte[] _datablockIn;
        private byte[] _datablockOut;
        private const int BLOCK_SIZE = 8192;
        private CancellationTokenSource _wtoken;
        private Header.Content _header;
        private Object _lockIn = new Object();
        private Object _lockOut = new Object();
        private Server _server;
    }
}
