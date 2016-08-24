using System;
using System.IO;
using System.Net;
using System.Linq;
using Sulakore.Habbo;
using Sulakore.Protocol;
using System.Net.Sockets;
using System.Collections.Generic;
using Sulakore.Protocol.Encryption;

namespace Sulakore.Communication
{
    public sealed class HConnection : HBase, IDisposable
    {
        #region Connection Events
        public override event EventHandler<EventArgs> OnConnected;
        public override event EventHandler<EventArgs> OnDisconnected;
        public override event EventHandler<DataToEventArgs> DataToClient;
        public override event EventHandler<DataToEventArgs> DataToServer;
        #endregion

        #region Private Fields
        private HTCPExt TCP;
        private Socket ClientS, ServerS;
        private int ToClientS, ToServerS, SocketCount;
        private byte[] ClientB, ServerB, CCache, SCache;
        private bool HasOfficialSocket, DisconnectAllowed;
        private readonly object DisposeLock, ResetHostLock, DisconnectLock, SendToClientLock, SendToServerLock;

        private static string HostsPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts";
        #endregion

        #region Public Properties
        public override bool IsConnected
        {
            get { return ServerS != null && ServerS.Connected; }
        }
        public override bool CaptureEvents { get; set; }

        private HRC4 _ServerDecrypt;
        public override HRC4 ServerDecrypt
        {
            get { return _ServerDecrypt; }
            set
            {
                if ((_ServerDecrypt = value) != null)
                    ResponseEncrypted = false;
            }
        }
        public override HRC4 ServerEncrypt { get; set; }

        private HRC4 _ClientDecrypt;
        public override HRC4 ClientDecrypt
        {
            get { return _ClientDecrypt; }
            set
            {
                if ((_ClientDecrypt = value) != null)
                    RequestEncrypted = false;
            }
        }
        public override HRC4 ClientEncrypt { get; set; }

        public int SocketSkip { get; set; }
        public HProtocols Protocol { get; private set; }

        public override int Port { get; protected set; }
        public override string Host { get; protected set; }
        public override string[] Addresses { get; protected set; }

        public override bool RequestEncrypted { get; protected set; }
        public override bool ResponseEncrypted { get; protected set; }
        #endregion

        #region Constructor(s)
        public HConnection(string Host, int Port)
        {
            DisposeLock = new object();
            ResetHostLock = new object();
            DisconnectLock = new object();
            SendToClientLock = new object();
            SendToServerLock = new object();

            this.Host = Host;
            this.Port = Port;
            ResetHost();

            Addresses = Dns.GetHostAddresses(Host).Select(IP => IP.ToString()).ToArray();
        }
        #endregion

        #region Public Methods
        public void Connect(bool Loopback = false)
        {
            if (Loopback)
            {
                string[] HostsL = File.ReadAllLines(HostsPath);
                if (!Array.Exists(HostsL, IP => Addresses.Contains(IP)))
                {
                    List<string> _GameIPs = Addresses.ToList(); if (!_GameIPs.Contains(Host)) _GameIPs.Add(Host);
                    string Mapping = string.Format("127.0.0.1\t\t{{0}}\t\t#{0}[{{1}}/{1}]", Host, _GameIPs.Count);
                    File.AppendAllLines(HostsPath, _GameIPs.Select(IP => string.Format(Mapping, IP, _GameIPs.IndexOf(IP) + 1)));
                }
            }

            (TCP = new HTCPExt(IPAddress.Any, Port)).Start();
            TCP.BeginAcceptSocket(SocketAccepted, null);
            DisconnectAllowed = true;
        }

        public override int SendToClient(byte[] Data)
        {
            if (ClientS != null && ClientS.Connected)
            {
                lock (SendToClientLock)
                {
                    if (ServerEncrypt != null)
                        Data = ServerEncrypt.SafeParse(Data);

                    return ClientS.Send(Data);
                }
            }
            else return 0;
        }
        public int SendToClient(ushort Header, params object[] Chunks)
        {
            return SendToClient(HMessage.Construct(Header, HDestinations.Client, Protocol, Chunks));
        }

        public override int SendToServer(byte[] Data)
        {
            if (IsConnected)
            {
                lock (SendToServerLock)
                {
                    if (ClientEncrypt != null)
                        Data = ClientEncrypt.SafeParse(Data);

                    return ServerS.Send(Data);
                }
            }
            else return 0;
        }
        public int SendToServer(ushort Header, params object[] Chunks)
        {
            return SendToServer(HMessage.Construct(Header, HDestinations.Server, Protocol, Chunks));
        }

        public void Dispose()
        {
            lock (DisposeLock)
            {
                #region Event Unsubscribing & Disconnecting
                SKore.Unsubscribe(ref OnConnected);
                SKore.Unsubscribe(ref DataToClient);
                SKore.Unsubscribe(ref DataToServer);
                SKore.Unsubscribe(ref OnDisconnected);
                Disconnect();
                #endregion

                Host = null;
                Addresses = null;
                Port = SocketSkip = 0;
                CaptureEvents = false;
            }
        }
        public void ResetHost()
        {
            lock (ResetHostLock)
            {
                if (Host == null || !File.Exists(HostsPath)) return;
                string[] HostsL = File.ReadAllLines(HostsPath).Where(Line => !Line.Contains(Host) && !Line.StartsWith("127.0.0.1")).ToArray();
                File.WriteAllLines(HostsPath, HostsL);
            }
        }
        public void Disconnect()
        {
            lock (DisconnectLock)
            {
                if (!DisconnectAllowed) return;
                DisconnectAllowed = false;

                if (ClientS != null)
                {
                    ClientS.Shutdown(SocketShutdown.Both);
                    ClientS.Close();
                    ClientS = null;
                }
                if (ServerS != null)
                {
                    ServerS.Shutdown(SocketShutdown.Both);
                    ServerS.Close();
                    ServerS = null;
                }
                ResetHost();
                if (TCP != null)
                {
                    TCP.Stop();
                    TCP = null;
                }
                Protocol = HProtocols.Unknown;
                ToClientS = ToServerS = SocketCount = 0;
                ClientB = ServerB = SCache = CCache = null;
                HasOfficialSocket = RequestEncrypted = ResponseEncrypted = false;
                ClientEncrypt = ClientDecrypt = ServerEncrypt = ServerDecrypt = null;
                if (OnDisconnected != null) OnDisconnected(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Private Methods
        private void SocketAccepted(IAsyncResult iAr)
        {
            try
            {
                if (++SocketCount == SocketSkip)
                {
                    TCP.EndAcceptSocket(iAr).Close();
                    TCP.BeginAcceptSocket(SocketAccepted, null);
                }
                else
                {
                    ClientS = TCP.EndAcceptSocket(iAr);
                    ServerS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    ServerS.BeginConnect(Addresses[0], Port, Connected, null);
                }
            }
            catch
            {
                if (TCP != null && TCP.Active)
                {
                    if (TCP.Pending()) TCP.EndAcceptSocket(iAr).Close();
                    TCP.BeginAcceptSocket(SocketAccepted, null);
                }
                else Disconnect();
            }
        }
        private void Connected(IAsyncResult iAr)
        {
            ServerS.EndConnect(iAr);
            ServerB = new byte[1024];
            ClientB = new byte[512];
            ReadClientData();
            ReadServerData();
        }

        private void ReadClientData()
        {
            if (ClientS != null && ClientS.Connected)
                ClientS.BeginReceive(ClientB, 0, ClientB.Length, SocketFlags.None, DataFromClient, null);
        }
        private void DataFromClient(IAsyncResult iAr)
        {
            try
            {
                if (ClientS == null) return;
                int Length = ClientS.EndReceive(iAr);

                if (Length > 0)
                {
                    byte[] Data = ByteUtils.CopyBlock(ClientB, 0, Length);
                    #region Official Socket Check
                    if (!HasOfficialSocket)
                    {
                        bool IsModern = false;
                        if (HasOfficialSocket = (Ancient.DecypherShort(Data, 3) == 206 || (IsModern = BigEndian.DecypherShort(Data, 4) == 4000)))
                        {
                            TCP.Stop(); TCP = null;
                            ResetHost();
                            Protocol = IsModern ? HProtocols.Modern : HProtocols.Ancient;
                            if (OnConnected != null) OnConnected(this, EventArgs.Empty);
                        }
                        else
                        {
                            SendToServer(Data);
                            return;
                        }
                    }
                    #endregion

                    try
                    {
                        #region Decrypt/Split
                        if (ClientDecrypt != null)
                            ClientDecrypt.Parse(Data);

                        if ((ToServerS + 1) == 4 && Protocol == HProtocols.Modern)
                        {
                            int DLength = BigEndian.DecypherInt(Data);
                            RequestEncrypted = (DLength > Data.Length - 4 || DLength < 6);
                        }

                        byte[][] Chunks = RequestEncrypted ? new byte[1][] { Data } : Chunks = ByteUtils.Split(ref SCache, Data, HDestinations.Server, Protocol);
                        #endregion
                        foreach (byte[] Chunk in Chunks)
                        {
                            ++ToServerS;
                            if (DataToServer == null) SendToServer(Chunk);
                            else
                            {

                                DataToEventArgs Args = new DataToEventArgs(Chunk, HDestinations.Server, ToServerS);
                                try { DataToServer(this, Args); }
                                catch
                                {
                                    SendToServer(Chunk);
                                    continue;
                                }
                                if (!Args.Skip) SendToServer(Args.Packet.ToBytes());
                            }
                            if (CaptureEvents && !RequestEncrypted) DataFromClient(Chunk);
                        }
                    }
                    catch { SendToServer(Data); }
                    ReadClientData();
                }
                else Disconnect();
            }
            catch { Disconnect(); }
        }

        private void ReadServerData()
        {
            if (IsConnected)
                ServerS.BeginReceive(ServerB, 0, ServerB.Length, SocketFlags.None, DataFromServer, null);
        }
        private void DataFromServer(IAsyncResult iAr)
        {
            try
            {
                if (ServerS == null) return;
                int Length = ServerS.EndReceive(iAr);

                if (Length > 0)
                {
                    byte[] Data = ByteUtils.CopyBlock(ServerB, 0, Length);
                    #region Official Socket Check
                    if (!HasOfficialSocket)
                    {
                        SendToClient(Data);
                        TCP.BeginAcceptSocket(SocketAccepted, null);
                        return;
                    }
                    #endregion

                    try
                    {
                        #region Decrypt/Split
                        if (ServerDecrypt != null)
                            ServerDecrypt.Parse(Data);

                        if ((ToClientS + 1) == 3 && Protocol == HProtocols.Modern)
                        {
                            int DLength = BigEndian.DecypherInt(Data);
                            ResponseEncrypted = (DLength > Data.Length - 4 || DLength < 6);
                        }

                        byte[][] Chunks = ResponseEncrypted ? new byte[1][] { Data } : ByteUtils.Split(ref CCache, Data, HDestinations.Client, Protocol);
                        #endregion
                        foreach (byte[] Chunk in Chunks)
                        {
                            ++ToClientS;
                            if (DataToClient == null) SendToClient(Chunk);
                            else
                            {
                                DataToEventArgs Args = new DataToEventArgs(Chunk, HDestinations.Client, ToClientS);
                                try { DataToClient(this, Args); }
                                catch
                                {
                                    SendToClient(Chunk);
                                    continue;
                                }
                                if (!Args.Skip) SendToClient(Args.Packet.ToBytes());
                            }
                            if (CaptureEvents && !ResponseEncrypted) DataFromServer(Chunk);
                        }
                    }
                    catch { SendToClient(Data); }
                    ReadServerData();
                }
                else Disconnect();
            }
            catch { Disconnect(); }
        }
        #endregion

        #region Nested Classes
        private class HTCPExt : TcpListener
        {
            public HTCPExt(IPEndPoint EndPoint)
                : base(EndPoint)
            { }
            public HTCPExt(IPAddress Address, int Port)
                : base(Address, Port)
            { }

            public new bool Active
            {
                get { return base.Active; }
            }
        }
        #endregion
    }
}