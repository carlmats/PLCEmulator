using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Common
{
    public class Range : IEnumerable<int>
    {
        private int _start, _end;
        public Range(int start, int end)
        {
            if (start > end)
                throw new ArgumentException("The end of the range can't have a lower value than the start of the range");

            _start = start;
            _end = end;
        }

        public Range(int startAndEnd)
        {
            _start = _end = startAndEnd;
        }

        public int Start
        {
            get => _start;
            set => _start = value;
        }

        public int End
        {
            get => _end;
            set => _end = value;
        }

        public void SetRange(int start, int end)
        {
            _start = start;
            _end = end;
        }

        public bool InRange(int value)
        {
            return value <= End && value >= Start ? true : false;
        }

        public int Count()
        {
            return End - Start;
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = Start; i <= End; i++)
                yield return i;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
