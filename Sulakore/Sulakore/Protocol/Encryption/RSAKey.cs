using System;

namespace Sulakore.Protocol.Encryption
{
    public class RSAKey
    {
        #region Private Fields
        private static Random ByteGen;
        #endregion

        #region Public Properties
        /// <summary>
        /// Public Exponent
        /// </summary>
        public BigInteger e { get; private set; }

        /// <summary>
        /// Public Modulus
        /// </summary>
        public BigInteger n { get; private set; }

        /// <summary>
        /// Private Exponent
        /// </summary>
        public BigInteger d { get; private set; }

        /// <summary>
        /// Secret Prime Factor ( P )
        /// </summary>
        public BigInteger p { get; private set; }

        /// <summary>
        /// Secret Prime Factor ( Q )
        /// </summary>
        public BigInteger q { get; private set; }

        /// <summary>
        /// d mod ( p - 1 )
        /// </summary>
        public BigInteger dmp1 { get; private set; }

        /// <summary>
        /// d mod ( q - 1 )
        /// </summary>
        public BigInteger dmq1 { get; private set; }

        /// <summary>
        /// (Inverse)q mod p
        /// </summary>
        public BigInteger iqmp { get; private set; }

        public bool CanEncrypt { get; private set; }
        public bool CanDecrypt { get; private set; }
        #endregion

        #region Constructor(s)
        static RSAKey()
        {
            ByteGen = new Random();
        }
        public RSAKey(BigInteger e, BigInteger n) : this(e, n, null, null, null, null, null, null) { }
        public RSAKey(BigInteger e, BigInteger n, BigInteger d) : this(e, n, d, null, null, null, null, null) { }
        public RSAKey(BigInteger e, BigInteger n, BigInteger d, BigInteger p, BigInteger q, BigInteger dmp1, BigInteger dmq1, BigInteger iqmp)
        {
            this.e = e;
            this.n = n;
            this.d = d;
            this.p = p;
            this.q = q;
            this.dmp1 = dmp1;
            this.dmq1 = dmq1;
            this.iqmp = iqmp;

            CanEncrypt = (e != null && n != null);
            CanDecrypt = (CanEncrypt && d != null);
        }
        #endregion

        #region Static Methods
        public static RSAKey Generate(int Exponent, int BitSize)
        {
            BigInteger _e, _n, _d, _p, _q, _dmp1, _dmq1, _iqmp;
            _e = Exponent;

            BigInteger phi, p1, q1;
            int QS = BitSize >> 1;
            do
            {
                do _p = BigInteger.genPseudoPrime(BitSize - QS, 6, ByteGen);
                while ((_p - 1).gcd(_e) != 1 && !_p.isProbablePrime(10));

                do _q = BigInteger.genPseudoPrime(QS, 6, ByteGen);
                while ((_q - 1).gcd(_e) != 1 && !_q.isProbablePrime(10));

                if (_p <= _q)
                {
                    BigInteger tmp_p = _p;
                    _p = _q; _q = tmp_p;
                }
                phi = (p1 = (_p - 1)) * (q1 = (_q - 1));
            }
            while (phi.gcd(_e) != 1);

            _n = _p * _q;
            _d = _e.modInverse(phi);
            _dmp1 = _d % p1;
            _dmq1 = _d % q1;
            _iqmp = _q.modInverse(_p);
            return new RSAKey(_e, _n, _d, _p, _q, _dmp1, _dmq1, _iqmp);
        }

        public static RSAKey ParsePublicKey(string e, string n)
        {
            return new RSAKey(new BigInteger(e, 16), new BigInteger(n, 16));
        }
        public static RSAKey ParsePrivateKey(string e, string n, string d)
        {
            return new RSAKey(new BigInteger(e, 16), new BigInteger(n, 16), new BigInteger(d, 16));
        }
        public static RSAKey ParsePrivateKey(string e, string n, string d, string p, string q, string dmp1, string dmq1, string iqmp)
        {
            return new RSAKey(new BigInteger(e, 16), new BigInteger(n, 16), new BigInteger(d, 16), new BigInteger(p, 16),
                new BigInteger(q, 16), new BigInteger(dmp1, 16), new BigInteger(dmq1, 16), new BigInteger(iqmp, 16));
        }
        #endregion

        #region Public Methods
        public void Encrypt(ref byte[] Data)
        {
            _Encrypt(DoPublic, ref Data, true);
        }
        public void Decrypt(ref byte[] Data)
        {
            _Decrypt(DoPrivate, ref Data);
        }

        public void Sign(ref byte[] Data)
        {
            _Encrypt(DoPrivate, ref Data, false);
        }
        public void Verify(ref byte[] Data)
        {
            _Decrypt(DoPublic, ref Data);
        }

        public int GetBlockSize()
        {
            return (n.bitCount() + 7) / 8;
        }
        #endregion

        #region Private Methods
        private void _Decrypt(Func<BigInteger, BigInteger> OP, ref byte[] Data)
        {
            int BS = GetBlockSize();
            Data = PKCS1Unpad(OP(new BigInteger(Data)).getBytes(), BS);
        }
        private void _Encrypt(Func<BigInteger, BigInteger> OP, ref byte[] Data, bool UseRandom)
        {
            int BS = GetBlockSize();
            Data = OP(new BigInteger(PKCS1Pad(Data, BS, UseRandom))).getBytes();
        }

        private byte[] PKCS1Unpad(byte[] Data, int Length)
        {
            int Offset = 0;
            while (Offset < Data.Length && Data[Offset] == 0) ++Offset;

            if (Data.Length - Offset != Length - 1 || Data[Offset] > 2)
                throw new Exception(string.Format("PKCS#1 UNPAD: Offset={0}, expected Data[Offset]==[0, 1, 2]; Got Data[Offset]={1:X}", Offset, Data[Offset]));
            else ++Offset;

            while (Data[Offset] != 0)
            {
                if (++Offset >= Data.Length)
                    throw new Exception(string.Format("PKCS#1 UNPAD: Offset={0}, Data[Offset - 1] != 0 (={1:X})", Offset, Data[Offset - 1]));
            }

            byte[] Out = new byte[(Data.Length - Offset) - 1];
            for (int j = 0; ++Offset < Data.Length; j++)
                Out[j] = Data[Offset];

            return Out;
        }
        private byte[] PKCS1Pad(byte[] Data, int Length, bool UseRandom = true)
        {
            byte[] Out = new byte[Length];
            for (int i = Data.Length - 1; (i >= 0 && Length > 11); )
                Out[--Length] = Data[i--];
            Out[--Length] = 0;

            while (Length > 2)
            {
                int x = 0;
                while (x == 0) x = UseRandom ? ByteGen.Next(0, 256) : 255;
                Out[--Length] = (byte)x;
            }
            Out[--Length] = (byte)(UseRandom ? 2 : 1);
            Out[--Length] = 0;
            return Out;
        }

        private BigInteger DoPublic(BigInteger x)
        {
            return x.modPow(e, n);
        }
        private BigInteger DoPrivate(BigInteger x)
        {
            if (p == null && q == null)
                return x.modPow(d, n);

            BigInteger xp = (x % p).modPow(dmp1, p);
            BigInteger xq = (x % q).modPow(dmq1, q);

            while (xp < xq) xp = xp + p;
            return ((((xp - xq) * (iqmp)) % p) * q) + xq;
        }

        private static BigInteger BigRandom(int BitSize)
        {
            return BigInteger.genPseudoPrime(BitSize, 6, ByteGen);
        }
        #endregion
    }
}