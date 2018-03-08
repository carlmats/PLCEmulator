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
        private bool _postByte;

        /// <summary>
        /// Defines a datablock of bytes where data of different tools are stored
        /// </summary>
        /// <param name="byteIndex">Continuous byte range for multiple devices of the same type</param>
        /// <param name="byteValue">Value of the byte when active</param>
        /// <param name="bytePost">If the byte is active or not, i.e. if the byte should be posted to the recieving system</param>
        public Datablock(Range byteIndex, byte byteValue, bool bytePost = false)
        {
            _range = byteIndex;
            _byteValue = byteValue;
            _postByte = bytePost;
        }

        public Range ByteIndex
        {
            get => _range;
            set => _range = value;
        }

        public byte ByteValue
        {
            get => _byteValue;
            set => _byteValue = value;
        }

        // Used to treat the byte as a bool (High / Low) 
        public bool BytePost
        {
            get => _postByte;
            set => _postByte = value;
        }
    }
}
