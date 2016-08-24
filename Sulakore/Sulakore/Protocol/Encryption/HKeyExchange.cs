using System;
using System.Text;
using System.Drawing;

namespace Sulakore.Protocol.Encryption
{
    public class HKeyExchange
    {
        #region Private Fields
        private readonly RSAKey RSA;
        private readonly int BitSize;
        private static Random ByteGen;
        #endregion

        #region Public Properties
        public BigInteger DHPrime { get; private set; }
        public BigInteger DHGenerator { get; private set; }
        public BigInteger DHPublic { get; private set; }
        public BigInteger DHPrivate { get; private set; }

        private string _SignedPrime;
        public string SignedPrime
        {
            get
            {
                if (!IsInitiator || !string.IsNullOrEmpty(_SignedPrime)) return _SignedPrime;

                byte[] PrimeAsBytes = Encoding.Default.GetBytes(DHPrime.ToString(10));
                RSA.Sign(ref PrimeAsBytes);

                return (_SignedPrime = BytesToHex(PrimeAsBytes).ToLower());
            }
        }

        private string _SignedGenerator;
        public string SignedGenerator
        {
            get
            {
                if (!IsInitiator || !string.IsNullOrEmpty(_SignedGenerator)) return _SignedGenerator;

                byte[] GeneratorAsBytes = Encoding.Default.GetBytes(DHGenerator.ToString(10));
                RSA.Sign(ref GeneratorAsBytes);

                return (_SignedGenerator = BytesToHex(GeneratorAsBytes).ToLower());
            }
        }

        private string _PublicKey;
        public string PublicKey
        {
            get
            {
                if (!string.IsNullOrEmpty(_PublicKey)) return _PublicKey;

                byte[] PublicKeyAsBytes = Encoding.Default.GetBytes(DHPublic.ToString(10));
                if (IsInitiator) RSA.Sign(ref PublicKeyAsBytes);
                else RSA.Encrypt(ref PublicKeyAsBytes);

                return (_PublicKey = BytesToHex(PublicKeyAsBytes).ToLower());
            }
        }

        public bool IsInitiator { get; private set; }
        public bool IsBannerHandshake { get; private set; }
        #endregion

        #region Constructor(s)
        static HKeyExchange()
        {
            ByteGen = new Random();
        }

        public HKeyExchange(int PublicExponent, string PublicModulus, int BitSize = 16)
            : this(new BigInteger(PublicExponent.ToString(), 16), new BigInteger(PublicModulus, 16), null, BitSize) { }
        public HKeyExchange(int PublicExponent, string PublicModulus, string PrivateExponent, int BitSize = 16)
            : this(new BigInteger(PublicExponent.ToString(), 16), new BigInteger(PublicModulus, 16), new BigInteger(PrivateExponent, 16), BitSize) { }

        public HKeyExchange(RSAKey Keys, int BitSize = 16)
            : this(Keys.e, Keys.n, Keys.d, BitSize) { }
        public HKeyExchange(BigInteger PublicExponent, BigInteger PublicModulus, int BitSize = 16)
            : this(PublicExponent, PublicModulus, null, BitSize) { }
        public HKeyExchange(BigInteger PublicExponent, BigInteger PublicModulus, BigInteger PrivateExponent, int BitSize = 16)
        {
            this.BitSize = BitSize;
            IsInitiator = (PrivateExponent != null);
            RSA = new RSAKey(PublicExponent, PublicModulus, PrivateExponent);

            if (IsInitiator)
            {
                do { DHPrime = BigInteger.genPseudoPrime(212, 6, ByteGen); }
                while (!DHPrime.isProbablePrime());

                do { DHGenerator = BigInteger.genPseudoPrime(212, 6, ByteGen); }
                while (DHGenerator == DHPrime || !DHPrime.isProbablePrime());

                if (DHGenerator > DHPrime)
                {
                    BigInteger DHGenShell = DHGenerator;
                    DHGenerator = DHPrime;
                    DHPrime = DHGenShell;
                }

                DHPrivate = new BigInteger(RandomHex(30), BitSize);
                DHPublic = DHGenerator.modPow(DHPrivate, DHPrime);
            }
        }
        #endregion

        #region Public Methods
        public byte[] GetSharedKey(string PublicKey)
        {
            if (!IsBannerHandshake)
            {
                byte[] PaddedPublicKeyAsBytes = HexToBytes(PublicKey);
                if (IsInitiator) RSA.Decrypt(ref PaddedPublicKeyAsBytes);
                else RSA.Verify(ref PaddedPublicKeyAsBytes);

                PublicKey = Encoding.Default.GetString(PaddedPublicKeyAsBytes);
            }

            BigInteger UnpaddedPublicKey = new BigInteger(PublicKey, 10);
            return UnpaddedPublicKey.modPow(DHPrivate, DHPrime).getBytes();
        }
        public void DoHandshake(Bitmap Banner, string Token)
        {
            IsBannerHandshake = true;
            byte[] BannerData = new byte[Banner.Width * Banner.Height * 4];
            for (int y = 0, i = 0; y < Banner.Height; y++)
            {
                for (int x = 0; x < Banner.Width; x++)
                {
                    int PixelARGB = Banner.GetPixel(x, y).ToArgb();
                    BannerData[i++] = (byte)((PixelARGB >> 24) & 255);
                    BannerData[i++] = (byte)((PixelARGB >> 16) & 255);
                    BannerData[i++] = (byte)((PixelARGB >> 8) & 255);
                    BannerData[i++] = (byte)(PixelARGB & 255);
                }
            }

            string BannerChunk = XOR(Decode(BannerData), Token);
            int BannerSize = BannerChunk[0];
            BannerChunk = BannerChunk.Substring(1);
            DHPrime = new BigInteger(BannerChunk.Substring(0, BannerSize), 10);

            BannerChunk = BannerChunk.Substring(BannerSize);
            BannerSize = BannerChunk[0];
            BannerChunk = BannerChunk.Substring(1);
            DHGenerator = new BigInteger(BannerChunk.Substring(0, BannerSize), 10);

            DHPrivate = new BigInteger(RandomHex(30), BitSize);
            DHPublic = DHGenerator.modPow(DHPrivate, DHPrime);
        }
        public void DoHandshake(string SignedPrime, string SignedGenerator)
        {
            if (IsInitiator) return;

            byte[] SignedPrimeAsBytes = HexToBytes(SignedPrime);
            RSA.Verify(ref SignedPrimeAsBytes);

            byte[] SignedGeneratorAsBytes = HexToBytes(SignedGenerator);
            RSA.Verify(ref SignedGeneratorAsBytes);

            DHPrime = new BigInteger(Encoding.Default.GetString(SignedPrimeAsBytes), 10);
            DHGenerator = new BigInteger(Encoding.Default.GetString(SignedGeneratorAsBytes), 10);

            if (DHPrime <= 2) throw new Exception("Prime cannot be <= 2!\nPrime: " + DHPrime.ToString());
            if (DHGenerator >= DHPrime) throw new Exception(string.Format("Generator cannot be >= Prime!\nPrime: {0}\nGenerator: {1}", DHPrime.ToString(), DHGenerator.ToString()));

            DHPrivate = new BigInteger(RandomHex(30), BitSize);
            DHPublic = DHGenerator.modPow(DHPrivate, DHPrime);
        }
        #endregion

        #region Private Methods
        private string Decode(byte[] Data)
        {
            int L7 = 0, L8 = 0;
            string Decoded = string.Empty;
            for (int i = 39; i < 69; i++)
            {
                for (int j = 4, k = 0; j < 84; j++)
                {
                    int Position = ((i + k) * 100 + j) * 4;
                    for (int l = 1; l < 4; l++)
                    {
                        L8 |= (Data[Position + l] & 1) << (7 - L7);
                        if (L7 == 7)
                        {
                            Decoded += (char)L8;
                            L7 = L8 = 0;
                        }
                        else L7++;
                    }
                    if (j % 2 == 0) k++;
                }
            }
            return Decoded;
        }
        private byte[] HexToBytes(string Hex)
        {
            int HexLength = Hex.Length;
            byte[] Data = new byte[HexLength / 2];
            for (int i = 0; i < HexLength; i += 2)
                Data[i / 2] = Convert.ToByte(Hex.Substring(i, 2), 16);
            return Data;
        }
        private string BytesToHex(byte[] Data)
        {
            return BitConverter.ToString(Data).Replace("-", string.Empty);
        }
        private string RandomHex(int Length = 16)
        {
            byte Generated = 0;
            string Hex = string.Empty;
            for (int i = 0; i < Length; i++)
            {
                Generated = (byte)ByteGen.Next(0, 256);
                Hex += Convert.ToString(Generated, 16);
            }
            return Hex;
        }
        private string XOR(string Value, string Token)
        {
            string Outcome = string.Empty;
            for (int i = 0, j = 0; i < Value.Length; i++)
            {
                Outcome += (char)(Value[i] ^ Token[j]);
                if (++j == Token.Length) j = 0;
            }
            return Outcome;
        }
        #endregion
    }
}