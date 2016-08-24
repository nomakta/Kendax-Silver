using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostExitEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[1] { "Header" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        #endregion

        public HostExitEventArgs(int Header)
        {
            this.Header = Header;
        }
        public static HostExitEventArgs CreateArguments(HMessage Packet)
        {
            return new HostExitEventArgs(HHeaders.Exit = Packet.Header) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0}", Header);
        }
    }
}