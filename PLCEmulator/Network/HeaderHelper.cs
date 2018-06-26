using System;

namespace PLCEmulator.Network
{
    public static class HeaderHelper
    {

        public static ContentOut header = new ContentOut();

        public struct ContentOut
        {
            public short Version { get; set; }
            public ushort Length { get; set; }
            public ushort Type { get; set; }
            public ushort Subsystem { get; set; }
            public ushort Number { get; set; }
            public int c_header_size => 10;
            public int c_max_num => 9999;


        };

        public struct ContentIn
        {
            public ushort requestUpdate;

            public ushort ackUpdated;

        };

        public static ContentIn ParseHeaderIn(byte[] datablock)
        {
            return new ContentIn()
            {
                requestUpdate = datablock[0],
                ackUpdated = datablock[1]
            };
        }

        public static void UpdateHeaderOut(ref byte[] datablock_out)
        {
            header.Version = 1;
            header.Type = 4;
            header.Subsystem = 1;
            header.Length = (ushort)datablock_out.Length;

            header.Number++;
            if (header.Number > header.c_max_num)
            {
                header.Number = 1;
            }

            var bytes = BitConverter.GetBytes(header.Version);
            datablock_out[0] = bytes[0];
            datablock_out[1] = bytes[1];

            bytes = BitConverter.GetBytes(header.Length);
            datablock_out[2] = bytes[1];
            datablock_out[3] = bytes[0];

            bytes = BitConverter.GetBytes(header.Type);
            datablock_out[4] = bytes[1];
            datablock_out[5] = bytes[0];

            bytes = BitConverter.GetBytes(header.Subsystem);
            datablock_out[6] = bytes[1];
            datablock_out[7] = bytes[0];

            bytes = BitConverter.GetBytes(header.Number);
            datablock_out[8] = bytes[1];
            datablock_out[9] = bytes[0];
        }
    }
}
