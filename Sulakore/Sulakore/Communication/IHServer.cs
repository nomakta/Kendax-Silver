using System;
using System.Net.Sockets;
using Sulakore.Protocol.Encryption;

namespace Sulakore.Communication
{
    public interface IHServer
    {
        event EventHandler<DataToEventArgs> DataToServer;

        bool RequestEncrypted { get; }

        HRC4 ClientEncrypt { get; set; }
        HRC4 ClientDecrypt { get; set; }

        int SendToClient(byte[] Data);
    }
}