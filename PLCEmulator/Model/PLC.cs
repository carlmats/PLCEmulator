using PLCEmulator.Common;
using PLCEmulator.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PLCEmulator.Network.Header;

namespace PLCEmulator.Model
{
    public class PLC : IIODevice
    {
        public List<IDevice> Devices { get; private set; }

        public Dictionary<Enum, Datablock> DataMapIn { get; protected set; } 

        public Dictionary<Enum, Datablock> DataMapOut { get; protected set; } 

        public PLC()
        {
            _datablockIn = new byte[BLOCK_SIZE];
            _datablockOut = new byte[BLOCK_SIZE];

            Devices = new List<IDevice>();
            Devices.Add(new DIOAD());
            DataMapIn = new Dictionary<Enum, Datablock>();
            DataMapOut = new Dictionary<Enum, Datablock>();

            // Read & write connected devices
            _wtoken = new CancellationTokenSource();
            Task.Factory.StartNew(ReadWriteDevice, _wtoken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // VDCOM connection
            _server = new Server("192.168.56.1", 2000, BLOCK_SIZE);
            _server.DataReceived += Server_DataReceived;


        }

        // Server communication
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
                    Network.Header.UpdateHeader(ref header, ref _datablockOut);
                }

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
                    ReadDevice(condevice);
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
                    WriteDevice(condevice);
                }
            }
        }

        private async void ReadWriteDevice()
        {
            while (_wtoken.IsCancellationRequested && Devices.Count > 0)
            {
                ReadDevice(this);
                WriteDevice(this);

                await Task.Delay(100);
            }

        }

        //private void ReadWriteVDCOM()
        //{
        //    var vdcom = new VDCOM();

        //    while (true)
        //    {
        //        TcpClient client = null;
        //        TcpListener listen = null;
        //        NetworkStream stream = null;
        //        Header header = new Header();

        //        try
        //        {
        //            vdcom.Connect(out listen, out client, out stream, "192.168.56.1", 2000);

        //            while (client.Connected)
        //            {
        //                if (ReadVDCOM(stream) > 0)
        //                {
        //                    VDCOM.UpdateHeader(ref header, ref _datablock_out);
        //                    WriteVDCOM(stream);
        //                }

        //                Thread.Sleep(100);
        //            }
        //        }
        //        catch (SocketException e)
        //        {
        //            Console.WriteLine("SocketException: {0}", e);
        //        }
        //        finally
        //        {
        //            listen.Stop();
        //        }
        //    }

        //}


        //private int ReadVDCOM(NetworkStream stream)
        //{
        //    lock (inlock)
        //    {
        //        return stream.Read(_datablock_in, 0, _datablock_in.Length);
        //    }
        //}

        //private void WriteVDCOM(NetworkStream stream)
        //{
        //    lock (outlock)
        //    {
        //        //_datablock_out[53] = 255;
        //        stream.Write(_datablock_out, 0, _datablock_out.Length);
        //    }
        //}

        private byte[] _datablockIn;
        private byte[] _datablockOut;

        private const int BLOCK_SIZE = 8192;

        private CancellationTokenSource _wtoken;

        private Object _lockIn = new Object();
        private Object _lockOut = new Object();

        private Server _server;
        private Header.Content header = new Header.Content();
    }
}
