using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCEmulator.Common
{
    // TODO: En separat för simpla bools och en för större värden
    // Kolla funktionalitet för read / write, finns bättre lösningar
    // subscriba på datamapIn blocken?
    public class DataBlock
    {

        public DataBlock(Range byteIndices, bool byteOwner = false, bool postByte = false)
        {
            _isByteOwner = byteOwner;
            _postByte = postByte;
            foreach (var index in byteIndices)
            {
                _dataDictionary.TryAdd(index, default(byte));
            }
        }

        public DataBlock(int byteIndex, bool byteOwner = false, bool postByte = false)
        {
            _isByteOwner = byteOwner;
            _postByte = postByte;
            _dataDictionary.TryAdd(byteIndex, default(byte));
        }

        public DataBlock(Range byteIndices, byte[] byteValues, bool byteOwner = false, bool postByte = false)
        {
         //   if (byteIndices.Count() != byteValues.Count())
        //        throw new ArgumentException("Byte count and range missmatch");

            _indices = byteIndices;
            _isByteOwner = byteOwner;
            _postByte = postByte;

            foreach (var index in byteIndices)
            {
                _dataDictionary.TryAdd(index, byteValues[index - _indices.Start]);
            }
        }

        public DataBlock(int byteIndex, byte byteValue, bool byteOwner = false, bool postByte = false)
        {
            _isByteOwner = byteOwner;
            _postByte = postByte;
            _dataDictionary.TryAdd(byteIndex, byteValue);
        }

        public IEnumerator<KeyValuePair<int, byte>> GetDataEntries()
        {
            return _dataDictionary.GetEnumerator();
        }

        public void UpdateData(int index, byte data)
        {
            _dataDictionary[index] = data;
            OnDataChanged(new DataChangedEventArgs(index, data));
        }

        public void AddData(int index, byte data)
        {
            if(_dataDictionary.TryAdd(index, data))
            {
                OnDataChanged(new DataChangedEventArgs(index, data));
            }
        }

        //TODO: Exceptions
        public Range GetRange()
        {
            return new Range(_dataDictionary.First().Key, _dataDictionary.Last().Key);
        }

        public bool PostByte
        {
            get => _postByte;
            set => _postByte = value;
        }

        public bool IsByteOwner
        {
            get => _isByteOwner;
            set => _isByteOwner = value;
        }

        protected virtual void OnDataChanged(DataChangedEventArgs e)
        {
            DataChanged?.Invoke(this, e);
        }

        public class DataChangedEventArgs : EventArgs
        {
            public DataChangedEventArgs(int key, byte data)
            {
                Key = key;
                NewData = data;
            }

            public int Key { get; set; }

            public byte NewData { get; set; }
        }

        private bool _isByteOwner;
        private bool _postByte;
        private Range _indices;
        private ConcurrentDictionary<int, byte> _dataDictionary = new ConcurrentDictionary<int, byte>();

        public event Action<DataBlock, DataChangedEventArgs> DataChanged = delegate { };

    }
}
