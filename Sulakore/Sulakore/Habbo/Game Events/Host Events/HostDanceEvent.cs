using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostDanceEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[2] { "Header", "Dance" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Dance",  Dance }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public HDances Dance { get; private set; }
        #endregion

        public HostDanceEventArgs(int Header, HDances Dance)
            : base()
        {
            this.Header = Header;
            this.Dance = Dance;
        }
        public static HostDanceEventArgs CreateArguments(HMessage Packet)
        {
            return new HostDanceEventArgs(HHeaders.Dance = Packet.Header, (HDances)Packet.ReadInt(0)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Dance: {1}", Header, Dance);
        }
    }
}