using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCEmulator.Network
{
    public sealed partial class Server
    {
        private sealed class Sender
        {
            internal void SendData(byte[] data)
            {
                _stream.Write(data, 0, data.Length);
            }

            internal Sender(NetworkStream stream, TcpClient client)
            {
                _stream = stream;
                _client = client;
            }


            private TcpClient _client;
            private NetworkStream _stream;
        }
    }
}
