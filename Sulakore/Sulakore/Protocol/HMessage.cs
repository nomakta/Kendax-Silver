using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Sulakore.Protocol
{
    public class HMessage
    {
        #region Private Fields
        private byte[] BCache;
        private ushort _Header;
        private bool LogWriting;
        private List<byte> Buffer;
        private string SCache, RawBody;
        private List<object> _Appended, _Prepended;
        #endregion

        #region Public Properties
        public ushort Header
        {
            get { return _Header; }
            set
            {
                if (!IsCorrupted && _Header != value)
                {
                    _Header = value;
                    Buffer.RemoveRange(0, 2);
                    Buffer.InsertRange(0, Protocol == HProtocols.Ancient ? Ancient.CypherShort(value) : BigEndian.CypherShort(value));
                    Reconstruct();
                }
            }
        }
        public int Position { get; set; }
        public int Length { get; private set; }
        public byte[] Body { get; private set; }
        public bool IsCorrupted { get; private set; }
        public HProtocols Protocol { get; private set; }
        public HDestinations Destination { get; private set; }

        public object[] Written
        {
            get
            {
                List<object> MergedWritten = new List<object>();
                MergedWritten.AddRange(_Prepended);
                MergedWritten.AddRange(_Appended);
                return MergedWritten.ToArray();
            }
        }
        public object[] Appended
        {
            get { return _Appended.ToArray(); }
        }
        public object[] Prepended
        {
            get { return _Prepended.ToArray(); }
        }
        #endregion

        #region Constructor(s)
        private HMessage()
        {
            Buffer = new List<byte>();
            _Appended = new List<object>();
            _Prepended = new List<object>();
        }

        public HMessage(byte[] Data)
            : this(Data, HDestinations.Unknown) { }

        public HMessage(string Packet)
            : this(ToBytes(Packet), HDestinations.Unknown) { }

        public HMessage(string Packet, HDestinations Destination)
            : this(ToBytes(Packet), Destination) { }

        public HMessage(byte[] Data, HDestinations Destination)
            : this()
        {
            if (Data == null) throw new NullReferenceException();
            if (Data.Length < 1) throw new Exception("The minimum amount of bytes required to initialize an HMessage instance is 1(One). If the amount of bytes passed is < 3(Three), and >= 1(One), it will be immediately be identified as a corrupted packet. { IsCorrupted = true }");

            this.Destination = Destination;
            bool HasByteZero = Data.Contains(byte.MinValue);
            bool IsAncientHeader = !HasByteZero && Data.Length == 2 && Data[1] != 1;

            if (!IsAncientHeader && Data.Length >= 6 && BigEndian.DecypherInt(Data) == Data.Length - 4)
            {
                Protocol = HProtocols.Modern;

                _Header = BigEndian.DecypherShort(Data, 4);
                Append(ByteUtils.CopyBlock(Data, 4, Data.Length - 4));

                if (Data.Length == 6)
                    LogWriting = true;
            }
            else if ((Destination == HDestinations.Server && IsAncientHeader) || (!HasByteZero && Data.Length >= 5 && Ancient.DecypherShort(Data, 1) == Data.Length - 3))
            {
                this.Destination = HDestinations.Server;
                Protocol = HProtocols.Ancient;

                _Header = Ancient.DecypherShort(Data, IsAncientHeader ? 0 : 3);
                Append(IsAncientHeader ? Data : ByteUtils.CopyBlock(Data, 3, Data.Length - 3));

                if (Data.Length == 5 || IsAncientHeader)
                    LogWriting = true;
            }
            else if (IsAncientHeader || (!HasByteZero && Data.Length >= 3 && Data[Data.Length - 1] == 1))
            {
                this.Destination = HDestinations.Client;
                Protocol = HProtocols.Ancient;

                if (IsAncientHeader) Data = new byte[3] { Data[0], Data[1], 1 };
                _Header = Ancient.DecypherShort(Data);
                Append(Data);

                if (Data.Length == 3 || IsAncientHeader)
                    LogWriting = true;
            }
            else
            {
                Body = Data;
                BCache = Data;
                IsCorrupted = true;
                Length = Data.Length;
                Buffer.AddRange(Data);
                SCache = ToString(Data);
            }
        }

        public HMessage(ushort Header, params object[] Chunks)
            : this(Header, HDestinations.Unknown, HProtocols.Modern, Chunks) { }

        public HMessage(ushort Header, HDestinations Destination, HProtocols Protocol, params object[] Chunks)
            : this(Construct(Header, Destination, Protocol, Chunks), Destination)
        {
            LogWriting = true;
            _Appended.AddRange(Chunks);
        }
        #endregion

        #region Reading Methods
        public int ReadInt()
        {
            int Index = Position;
            int Value = ReadInt(ref Index);
            Position = Index;
            return Value;
        }
        public int ReadInt(int Index)
        {
            return ReadInt(ref Index);
        }
        public int ReadInt(ref int Index)
        {
            if (IsCorrupted) return 0;
            switch (Protocol)
            {
                case HProtocols.Modern: return BigEndian.DecypherInt(Body[Index++], Body[Index++], Body[Index++], Body[Index++]);
                case HProtocols.Ancient:
                {
                    int Value = Ancient.DecypherInt(Body, Index);
                    Index += Ancient.CypherInt(Value).Length;
                    return Value;
                }
                default: return 0;
            }
        }

        public int ReadShort()
        {
            int Index = Position;
            int Value = ReadShort(ref Index);
            Position = Index;
            return Value;
        }
        public int ReadShort(int Index)
        {
            return ReadShort(ref Index);
        }
        public int ReadShort(ref int Index)
        {
            if (IsCorrupted) return 0;
            switch (Protocol)
            {
                case HProtocols.Modern: return BigEndian.DecypherShort(Body[Index++], Body[Index++]);
                case HProtocols.Ancient: return Ancient.DecypherShort(Body[Index++], Body[Index++]);
                default: return 0;
            }
        }

        public bool ReadBool()
        {
            int Index = Position;
            bool Value = ReadBool(ref Index);
            Position = Index;
            return Value;
        }
        public bool ReadBool(int Index)
        {
            return ReadBool(ref Index);
        }
        public bool ReadBool(ref int Index)
        {
            if (IsCorrupted) return false;
            switch (Protocol)
            {
                case HProtocols.Modern: return Body[Index++] == 1;
                case HProtocols.Ancient: return Body[Index++] == 'I';
                default: return false;
            }
        }

        public string ReadString()
        {
            int Index = Position;
            string Value = ReadString(ref Index);
            Position = Index;
            return Value;
        }
        public string ReadString(int Index)
        {
            return ReadString(ref Index);
        }
        public string ReadString(ref int Index)
        {
            if (IsCorrupted) return string.Empty;
            if (Protocol == HProtocols.Modern || (Protocol == HProtocols.Ancient && Destination == HDestinations.Server))
            {
                int SLength = ReadShort(ref Index);
                byte[] SData = ByteUtils.CopyBlock(Body, (Index += SLength) - SLength, SLength);
                return Encoding.Default.GetString(SData);
            }
            else if (Protocol == HProtocols.Ancient && Destination == HDestinations.Client)
            {
                string Chunk = RawBody.Substring(Index).Split((char)2)[0];
                Index += Chunk.Length + 1;
                return Chunk;
            }
            else return string.Empty;
        }
        #endregion

        #region Writing Methods
        public void Append(int Value)
        {
            Append(new object[1] { Value });
        }
        public void Append(bool Value)
        {
            Append(new object[1] { Value });
        }
        public void Append(string Value)
        {
            Append(new object[1] { Value });
        }
        public void Append(params object[] Chunks)
        {
            if (IsCorrupted) return;
            if (LogWriting) _Appended.AddRange(Chunks);
            byte[] Constructed = ConstructBody(Destination, Protocol, Chunks);
            if (Protocol == HProtocols.Ancient && Destination == HDestinations.Client)
            {
                Buffer.InsertRange(Buffer.Count - 1, Constructed);
                Reconstruct();
            }
            else Append(Constructed);
        }

        public void Prepend(int Value)
        {
            Prepend(new object[1] { Value });
        }
        public void Prepend(bool Value)
        {
            Prepend(new object[1] { Value });
        }
        public void Prepend(string Value)
        {
            Prepend(new object[1] { Value });
        }
        public void Prepend(params object[] Chunks)
        {
            if (IsCorrupted) return;
            if (LogWriting) _Prepended.AddRange(Chunks);
            byte[] Constructed = ConstructBody(Destination, Protocol, Chunks);
            Prepend(Constructed);
        }
        #endregion

        #region Private Methods
        private void Reconstruct()
        {
            BCache = null;
            SCache = null;

            Length = Buffer.Count;
            if (Body == null || Body.Length != Length - 2)
            {
                Body = ByteUtils.CopyBlock(Buffer.ToArray(), 2, Length - 2);
                RawBody = Encoding.Default.GetString(Body);
            }
        }
        private void Append(params byte[] Chunk)
        {
            Buffer.AddRange(Chunk);
            Reconstruct();
        }
        private void Prepend(params byte[] Chunk)
        {
            Buffer.InsertRange(2, Chunk);
            Position += Chunk.Length;
            Reconstruct();
        }
        #endregion

        #region Instance Formatters
        public byte[] ToBytes()
        {
            return BCache ?? (BCache = Construct(_Header, Destination, Protocol, Body));
        }
        public override string ToString()
        {
            return SCache ?? (SCache = ToString(ToBytes()));
        }
        #endregion

        #region Static Methods
        public static byte[] ToBytes(string Packet)
        {
            for (int i = 0; i <= 13; i++)
                Packet = Packet.Replace("[" + i + "]", ((char)i).ToString());
            return Encoding.Default.GetBytes(Packet);
        }
        public static string ToString(byte[] Packet)
        {
            string Result = Encoding.Default.GetString(Packet);
            for (int i = 0; i <= 13; i++)
                Result = Result.Replace(((char)i).ToString(), "[" + i + "]");
            return Result;
        }

        public static byte[] Construct(ushort Header, params object[] Chunks)
        {
            return Construct(Header, HDestinations.Unknown, HProtocols.Modern, Chunks);
        }
        public static byte[] ConstructBody(HDestinations Destination, HProtocols Protocol, params object[] Chunks)
        {
            if (Protocol == HProtocols.Unknown) throw new Exception("You must specify a supported HProtocols value for this method. ( Ancient / Modern )");
            if (Protocol == HProtocols.Ancient && Destination == HDestinations.Unknown) throw new Exception("Cannot construct the body of an Ancient type packet without a valid HDestinations value. ( Client / Server )");

            List<byte> Buffer = new List<byte>();
            bool IsAncient = Protocol == HProtocols.Ancient;
            for (int i = 0; i < Chunks.Length; i++)
            {
                object Chunk = Chunks[i];
                if (Chunk == null)
                    throw new NullReferenceException(string.Format("Unable to encode a null object. {{ Index = {0} }}", i));

                if (Chunk is byte[]) Buffer.AddRange((byte[])Chunk);
                else
                {
                    switch (Type.GetTypeCode(Chunk.GetType()))
                    {
                        case TypeCode.Int32:
                        {
                            int Value = (int)Chunk;
                            Buffer.AddRange(IsAncient ? Ancient.CypherInt(Value) : BigEndian.CypherInt(Value));
                            break;
                        }
                        default:
                        case TypeCode.String:
                        {
                            string Value = Chunk.ToString();
                            if (!IsAncient || Destination == HDestinations.Server)
                            {
                                Buffer.AddRange(IsAncient ? Ancient.CypherShort((ushort)Value.Length) : BigEndian.CypherShort((ushort)Value.Length));
                                Buffer.AddRange(Encoding.Default.GetBytes(Value));
                            }
                            else
                            {
                                Buffer.AddRange(Encoding.Default.GetBytes(Value));
                                Buffer.Add(2);
                            }
                            break;
                        }
                        case TypeCode.Boolean:
                        {
                            bool Value = (bool)Chunk;
                            Buffer.Add(IsAncient ? (byte)(Value ? 73 : 72) : Convert.ToByte(Value));
                            break;
                        }
                        case TypeCode.Byte:
                        {
                            byte Value = (byte)Chunk;
                            Buffer.Add(Value);
                            break;
                        }
                    }
                }
            }
            return Buffer.ToArray();
        }
        public static byte[] Construct(ushort Header, HDestinations Destination, HProtocols Protocol, params object[] Chunks)
        {
            if (Protocol == HProtocols.Unknown) throw new Exception("You must specify a supported HProtocols value for this method. ( Ancient / Modern )");
            if (Protocol == HProtocols.Ancient && Destination == HDestinations.Unknown) throw new Exception("Cannot construct the body of an Ancient type packet without a valid HDestinations value. ( Client / Server )");

            List<byte> Buffer = new List<byte>();
            bool IsAncient = Protocol == HProtocols.Ancient;
            if (IsAncient && Destination == HDestinations.Server) Buffer.Add(64);
            Buffer.AddRange(IsAncient ? Ancient.CypherShort(Header) : BigEndian.CypherShort(Header));

            Buffer.AddRange(ConstructBody(Destination, Protocol, Chunks));

            if (!IsAncient || Destination == HDestinations.Server)
                Buffer.InsertRange(IsAncient ? 1 : 0, IsAncient ? Ancient.CypherShort((ushort)(Buffer.Count - 1)) : BigEndian.CypherInt(Buffer.Count));
            else if (Buffer[Buffer.Count - 1] != 1) Buffer.Add(1);

            return Buffer.ToArray();
        }
        #endregion
    }
}