using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostKickedEventArgs : EventArgs, IHabboEvent
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

        public HostKickedEventArgs(int Header)
            : base()
        {
            this.Header = Header;
        }
        public static HostKickedEventArgs CreateArguments(HMessage Packet)
        {
            return new HostKickedEventArgs(Packet.Header) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Client) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0}", Header);
        }
    }
}