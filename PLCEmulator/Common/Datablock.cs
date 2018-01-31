using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Common
{
    public class Datablock
    {
        private Range _range;
        private byte _byteValue;
        public Datablock(Range range, byte byteValue = 0)
        {
            _range = range;
            _byteValue = byteValue;
        }

        public Range Range
        {
            get => _range;
            set => _range = value;
        }

        public byte ByteValue
        {
            get => _byteValue;
            set => _byteValue = value;
        }
    }
}
