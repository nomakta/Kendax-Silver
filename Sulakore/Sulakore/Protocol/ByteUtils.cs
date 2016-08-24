using System.Linq;
using System.Collections.Generic;

namespace Sulakore.Protocol
{
    public static class ByteUtils
    {
        private static readonly object SplitLock;
        static ByteUtils()
        {
            SplitLock = new object();
        }

        public static byte[] Merge(byte[] Base, params byte[][] Chunks)
        {
            List<byte> Data = new List<byte>();
            Data.AddRange(Base);
            foreach (byte[] Chunk in Chunks)
                Data.AddRange(Chunk);
            return Data.ToArray();
        }
        public static byte[][] Split(ref byte[] Cache, byte[] Data, HDestinations Destination, HProtocols Protocol)
        {
            lock (SplitLock)
            {
                if (Cache != null)
                {
                    Data = Merge(Cache, Data);
                    Cache = null;
                }

                List<byte[]> Chunks = new List<byte[]>();
                if (Protocol == HProtocols.Ancient && Destination == HDestinations.Client)
                {
                    if (!Data.Contains((byte)1)) Cache = Data;
                    else
                    {
                        List<byte> Buffer = new List<byte>();
                        foreach (byte Value in Data)
                        {
                            Buffer.Add(Value);
                            if (Value == 1)
                            {
                                Chunks.Add(Buffer.ToArray());
                                Buffer.Clear();
                            }
                        }
                        if (Buffer.Count > 0) Cache = Buffer.ToArray();
                    }
                }
                else
                {
                    bool IsAncient = Protocol == HProtocols.Ancient;
                    int Offset = IsAncient ? 3 : 4;
                    int Length = IsAncient ? Ancient.DecypherShort(Data, 1) : BigEndian.DecypherInt(Data);
                    if (Length == Data.Length - Offset)
                        Chunks.Add(Data);
                    else
                    {
                        do
                        {
                            if (Length > Data.Length - Offset) { Cache = Data; break; }
                            Chunks.Add(CutBlock(ref Data, 0, Length + Offset));
                            if (Data.Length >= Offset)
                                Length = IsAncient ? Ancient.DecypherShort(Data, 1) : BigEndian.DecypherInt(Data);
                        }
                        while (Data.Length != 0);
                    }
                }
                return Chunks.ToArray();
            }
        }

        public static byte[] CopyBlock(byte[] Data, int Offset, int Length)
        {
            Length = (Length > Data.Length) ? Data.Length : Length < 0 ? 0 : Length;
            Offset = Offset + Length >= Data.Length ? Offset = Data.Length - Length : Offset < 0 ? 0 : Offset;

            byte[] Chunk = new byte[Length];
            for (int i = 0; i < Length; i++) Chunk[i] = Data[Offset++];
            return Chunk;
        }
        public static byte[] CutBlock(ref byte[] Data, int Offset, int Length)
        {
            Length = (Length > Data.Length) ? Data.Length : Length < 0 ? 0 : Length;
            Offset = Offset + Length >= Data.Length ? Offset = Data.Length - Length : Offset < 0 ? 0 : Offset;

            byte[] Chunk = new byte[Length];
            byte[] Trimmed = new byte[Data.Length - Length];
            for (int i = 0, j = Offset; i < Length; i++) Chunk[i] = Data[j++];
            for (int i = 0, j = 0; i < Data.Length; i++)
            {
                if (i < Offset || i >= Offset + Length)
                    Trimmed[j++] = Data[i];
            }
            Data = Trimmed;
            return Chunk;
        }
    }
}