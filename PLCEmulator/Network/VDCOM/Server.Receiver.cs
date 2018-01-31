using PLCEmulator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLCEmulator.Network.VDCOM
{
    public sealed partial class Server
    {
        private sealed class Receiver
        {
            internal event EventHandler<DataReceivedEventArgs> DataReceived;

            internal Receiver(NetworkStream stream, TcpClient client, int blockSize)
            {
                _stream = stream;
                //_stream.ReadTimeout = 5000;
                _client = client;
                _data = new byte[blockSize];
                _blockSize = blockSize;
                _wtoken = new CancellationTokenSource();

                Task.Factory.StartNew(Run, _wtoken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }


            private void Run()
            {
                while(_client.Connected && !_wtoken.IsCancellationRequested)
                {
                    if (_stream.Read(_data, 0, _data.Length) > 0)
                    {
                        DataReceived?.BeginInvoke(this, new DataReceivedEventArgs(_data), EndAsyncEvent, null);
                    }
                }
            }

            private void EndAsyncEvent(IAsyncResult iar)
            {
                try
                {
                    DataReceived.EndInvoke(iar);
                }
                catch
                {
                    Console.WriteLine("An event listener blew up.. RIP");
                }
            }

            private TcpClient _client;
            private NetworkStream _stream;
            private CancellationTokenSource _wtoken;
            private byte[] _data;
            private int _blockSize;
        }
    }
}
