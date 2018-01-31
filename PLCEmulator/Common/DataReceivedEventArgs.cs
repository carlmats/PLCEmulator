using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Common
{
    public class DataReceivedEventArgs : EventArgs
    {
        private readonly byte[] _data;
        public byte[] Data => _data;

        public DataReceivedEventArgs(byte[] data)
        {
            _data = data;
        }
    }
}
