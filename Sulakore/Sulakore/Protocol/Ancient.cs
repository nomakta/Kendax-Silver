namespace Sulakore.Protocol
{
    public static class Ancient
    {
        public static byte[] CypherShort(ushort Value)
        {
            return new byte[2] { (byte)(64 + (Value >> 6 & 63)), (byte)(64 + (Value >> 0 & 63)) };
        }
        public static byte[] CypherShort(byte[] Base, int Offset, ushort Value)
        {
            Offset = Offset > Base.Length ? Base.Length : Offset < 0 ? 0 : Offset;

            byte[] Data = new byte[Base.Length + 2];
            for (int i = 0, j = 0; j < Data.Length; j++)
            {
                if (j != Offset) Data[j] = Base[i++];
                else
                {
                    byte[] ToInsert = CypherShort(Value);
                    Data[j++] = ToInsert[0];
                    Data[j] = ToInsert[1];
                }
            }
            return Data;
        }
        public static ushort DecypherShort(byte[] Data)
        {
            return DecypherShort(Data, 0);
        }
        public static ushort DecypherShort(string Encoded)
        {
            return DecypherShort(new byte[2] { (byte)Encoded[0], (byte)Encoded[1] }, 0);
        }
        public static ushort DecypherShort(byte[] Data, int Offset)
        {
            return (ushort)(Data.Length > 1 ? (Data[Offset + 1] - 64 + (Data[Offset] - 64) * 64) : 0);
        }
        public static ushort DecypherShort(byte First, byte Second)
        {
            return DecypherShort(new byte[2] { First, Second }, 0);
        }

        public static byte[] CypherInt(int Value)
        {
            int Length = 1;
            int NonNegative = Value < 0 ? -(Value) : Value;
            byte[] Buffer = new byte[6] { (byte)(64 + (NonNegative & 3)), 0, 0, 0, 0, 0 };

            for (NonNegative >>= 2; NonNegative != 0; NonNegative >>= 6, Length++)
                Buffer[Length] = (byte)(64 + (NonNegative & 63));

            Buffer[0] = (byte)(Buffer[0] | Length << 3 | (Value >= 0 ? 0 : 4));

            byte[] ZerosTrimmed = new byte[Length];
            for (int i = 0; i < Length; i++)
                ZerosTrimmed[i] = Buffer[i];

            return ZerosTrimmed;
        }
        public static byte[] CypherInt(byte[] Base, int Offset, int Value)
        {
            Offset = Offset > Base.Length ? Base.Length : Offset < 0 ? 0 : Offset;

            byte[] ToInsert = CypherInt(Value);
            byte[] Data = new byte[Base.Length + ToInsert.Length];
            for (int i = 0, j = 0; j < Data.Length; j++)
            {
                if (j != Offset) Data[j] = Base[i++];
                else
                {
                    for (int k = 0, l = j; k < ToInsert.Length; k++, l++)
                        Data[l] = ToInsert[k];
                    j += ToInsert.Length - 1;
                }
            }
            return Data;
        }
        public static int DecypherInt(byte[] Data)
        {
            return DecypherInt(Data, 0);
        }
        public static int DecypherInt(string Encoded)
        {
            byte[] Data = new byte[Encoded.Length];
            for (int i = 0; i < Encoded.Length; i++)
                Data[i] = (byte)Encoded[i];
            return DecypherInt(Data, 0);
        }
        public static int DecypherInt(byte[] Data, int Offset)
        {
            int Length = (Data[Offset] >> 3) & 7;
            int Decoded = Data[Offset] & 3;
            bool IsNegative = (Data[Offset] & 4) == 4;
            for (int i = 1, j = Offset + 1, k = 2; i < Length; i++, j++)
            {
                if (Length > Data.Length - Offset) break;
                Decoded |= (Data[j] & 63) << k;
                k = 2 + (6 * i);
            }
            return IsNegative ? -(Decoded) : Decoded;
        }
        public static int DecypherInt(byte First, params byte[] Data)
        {
            byte[] Buffer = new byte[Data.Length + 1];
            Buffer[0] = First;
            for (int i = 0; i < Buffer.Length; i++)
                Buffer[i + 1] = Data[i];

            return DecypherInt(Buffer, 0);
        }
    }
}