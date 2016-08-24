using System;

namespace Sulakore.Protocol.Encryption
{
    public class HRC4
    {
        #region Private Fields
        private int i, j;
        private int[] Table;
        #endregion

        #region Constructor(s)
        public HRC4(int Key)
        {
            Key = Key < 0 ? 0 : Key;
            byte[] NKey = new byte[1] { (byte)Key };
            if (Key > byte.MaxValue)
                NKey = Key > ushort.MaxValue ? BigEndian.CypherInt(Key) : BigEndian.CypherShort((ushort)Key);
            Initialize(NKey);
        }
        public HRC4(byte[] Key)
        {
            Initialize(Key);
        }
        #endregion

        #region Private Methods
        private void Initialize(byte[] Key)
        {
            Table = new int[256];
            for (int k = 0; k < 256; k++) Table[k] = k;
            for (int k = 0, EnX = 0; k < 256; k++)
                Swap(k, EnX = (((EnX + Table[k]) + (Key[k % Key.Length])) % 256));
        }
        private void Swap(int A, int B)
        {
            int Save = Table[A];
            Table[A] = Table[B];
            Table[B] = Save;
        }
        #endregion

        #region Public Methods
        public void Parse(byte[] Data)
        {
            for (int k = 0; k < Data.Length; k++)
            {
                Swap(i = (++i % 256), j = ((j + Table[i]) % 256));
                Data[k] ^= (byte)(Table[(Table[i] + Table[j]) % 256]);
            }
        }
        public byte[] SafeParse(byte[] Data)
        {
            byte[] ShallowCopy = new byte[Data.Length];
            Data.CopyTo(ShallowCopy, 0);
            Parse(ShallowCopy);
            return ShallowCopy;
        }
        #endregion
    }
}