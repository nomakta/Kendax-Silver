namespace Sulakore.Protocol
{
    public static class BigEndian
    {
        public static byte[] CypherShort(ushort Value)
        {
            return new byte[2] { (byte)(Value >> 8), (byte)Value };
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
            return (ushort)((Data[Offset] | Data[Offset + 1]) < 0 ? -1 : ((Data[Offset] << 8) + Data[Offset + 1]));
        }
        public static ushort DecypherShort(byte First, byte Second)
        {
            return DecypherShort(new byte[2] { First, Second }, 0);
        }

        public static byte[] CypherInt(int Value)
        {
            return new byte[4] { (byte)(Value >> 24), (byte)(Value >> 16), (byte)(Value >> 8), (byte)Value };
        }
        public static byte[] CypherInt(byte[] Base, int Offset, int Value)
        {
            Offset = Offset > Base.Length ? Base.Length : Offset < 0 ? 0 : Offset;

            byte[] Data = new byte[Base.Length + 4];
            for (int i = 0, j = 0; j < Data.Length; j++)
            {
                if (j != Offset) Data[j] = Base[i++];
                else
                {
                    byte[] ToInsert = CypherInt(Value);
                    Data[j++] = ToInsert[0];
                    Data[j++] = ToInsert[1];
                    Data[j++] = ToInsert[2];
                    Data[j] = ToInsert[3];
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
            return DecypherInt(new byte[4] { (byte)Encoded[0], (byte)Encoded[1], (byte)Encoded[2], (byte)Encoded[3] }, 0);
        }
        public static int DecypherInt(byte[] Data, int Offset)
        {
            return (Data[Offset] | Data[Offset + 1] | Data[Offset + 2] | Data[Offset + 3]) < Offset ? -1 : ((Data[Offset] << 24) + (Data[Offset + 1] << 16) + (Data[Offset + 2] << 8) + Data[Offset + 3]);
        }
        public static int DecypherInt(byte First, byte Second, byte Third, byte Fourth)
        {
            return DecypherInt(new byte[4] { First, Second, Third, Fourth }, 0);
        }
    }
}