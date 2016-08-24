using System.Net;
using Sulakore.Communication;

namespace Sulakore.Habbo
{
    public interface IPlayerSession : IHServer, IHClient
    {
        CookieContainer Cookies { get; }

        string SSOTicket { get; }
        string PlayerName { get; }
        int PlayerID { get; }

        bool Login();
    }
}