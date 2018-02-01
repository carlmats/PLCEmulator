using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Common
{
    public class Range
    {
        private int _start, _end;
        public Range(int start, int end)
        {
            _start = start;
            _end = end;
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
    }
}
