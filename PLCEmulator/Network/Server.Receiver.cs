using PLCEmulator.Common;
using PLCEmulator.Debug;
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
        private sealed class Receiver
        {
            internal event EventHandler<DataReceivedEventArgs> DataReceived;

            internal Receiver(NetworkStream stream, TcpClient client, int blockSize)
            {
                _stream = stream;
                _client = client;
                _data = new byte[blockSize];
                _blockSize = blockSize;
                _wtoken = new CancellationTokenSource();

                Task.Factory.StartNew(Run, _wtoken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }


            private void Run()
            {
                while (_client.Connected && !_wtoken.IsCancellationRequested)
                {
                    try
                    {
                        if (_stream.CanRead && _stream.Read(_data, 0, _data.Length) > 0)
                        {
                            DataReceived?.BeginInvoke(this, new DataReceivedEventArgs(_data), EndAsyncEvent, null);
                        }
                    }
                    catch
                    {
                        if(_wtoken.IsCancellationRequested)
                            Logger.Instance.WriteLog("Aborted read due to cancellation request");
                        else
                            Logger.Instance.WriteLog("Aborted read due to an unkown exception");
                    }
                }
            }

            internal void Dispose()
            {
                _wtoken?.Cancel();
                Logger.Instance.WriteLog("Disposing reciever");
            }

            private void EndAsyncEvent(IAsyncResult iar)
            {
                try
                {
                    DataReceived.EndInvoke(iar);
                }
                catch
                {
                    Logger.Instance.WriteLog("An event listener blew up.. RIP");
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
