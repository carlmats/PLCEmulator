using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCEmulator.Network.VDCOM
{
    public sealed partial class Server : IDisposable
    {
        public void SendData(byte[] data)
        {
            _sender.SendData(data);
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public Server(string ip, int port, int blockSize)
        {
            Task.Run(() => Start(ip,port,blockSize));
        }

        private async void Start(string ip, int port, int blockSize)
        {
            _listner = new TcpListener(IPAddress.Parse(ip), port);
            _listner.Start();

            while (!_listner.Pending())
            {
                Console.WriteLine("Still listening.. Checking again in 5 seconds");
                await Task.Delay(5000);
            }
            _client = _listner.AcceptTcpClient();
            _stream = _client.GetStream();

            _receiver = new Receiver(_stream, _client, blockSize);
            _sender = new Sender(_stream, _client);

            _receiver.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            var handler = DataReceived;
            if (handler != null) DataReceived(this, e);  // re-raise event
        }

        public void Dispose()
        {
           
        }

        private TcpClient _client;
        private NetworkStream _stream;
        private TcpListener _listner;
        private Receiver _receiver;
        private Sender _sender;
    }
}
