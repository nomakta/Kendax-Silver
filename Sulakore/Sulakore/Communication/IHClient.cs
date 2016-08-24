using System;
using System.Net.Sockets;
using Sulakore.Protocol.Encryption;

namespace Sulakore.Communication
{
    public interface IHClient
    {
        event EventHandler<EventArgs> OnConnected;
        event EventHandler<EventArgs> OnDisconnected;
        event EventHandler<DataToEventArgs> DataToClient;

        int Port { get; }
        string Host { get; }
        string[] Addresses { get; }

        bool IsConnected { get; }
        bool ResponseEncrypted { get; }

        HRC4 ServerDecrypt { get; set; }
        HRC4 ServerEncrypt { get; set; }

        int SendToServer(byte[] Data);
    }
}