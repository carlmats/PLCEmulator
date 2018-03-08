using PLCEmulator.Common;
using PLCEmulator.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCEmulator.Network
{
    public sealed partial class Server : IDisposable
    {
        public void SendData(byte[] data)
        {
            _sender?.SendData(data);
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public async Task<bool> Start(string ip, int port, int blockSize)
        {
            Logger.Instance.WriteLog("Attempting to establish connection with PLC");

           _listner = new TcpListener(IPAddress.Parse(ip), port);

            try
            {
                _listner.Start();
                _client = await Listen(ip, port, blockSize);
            }
            catch(SocketException se)
            {
                Logger.Instance.WriteLog(se.Message);
            }

            if(!_abort && _client != null)
            {
                _stream = _client.GetStream();
                _receiver = new Receiver(_stream, _client, blockSize);
                _sender = new Sender(_stream, _client);
                _receiver.DataReceived += OnDataReceived;
                Logger.Instance.WriteLog("Client connected");

                return true;
            }
            else
            {
                if (_abort)
                    Logger.Instance.WriteLog("Aborted listen due to cancellation request");

                return false;
            }

        }

        private async Task<TcpClient> Listen(string ip, int port, int blockSize)
        {
            while (!_abort && !_listner.Pending())
            {
                Logger.Instance.WriteLog("Still listening.. Checking again in 5 seconds");
                await Task.Delay(5000);
            }

            if (!_abort)
                return _listner.AcceptTcpClient();
            else
                return null;

        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            var handler = DataReceived;
            if (handler != null) DataReceived(this, e);  // re-raise event
        }

        public bool IsConnected()
        {
            if (_client != null)
                return _client.Connected;

            return false;
        }
        public void Dispose()
        {
            Logger.Instance.WriteLog("Disposing server");

            _abort = true;
            _receiver?.Dispose();
            _client?.Close();
            _stream?.Close();
            _listner?.Stop();

            _receiver = null;
            _client = null;
            _stream = null;
            _listner = null;
        }

        private TcpClient _client;
        private NetworkStream _stream;
        private TcpListener _listner;
        private Receiver _receiver;
        private Sender _sender;
        private bool _abort;

    }
}
