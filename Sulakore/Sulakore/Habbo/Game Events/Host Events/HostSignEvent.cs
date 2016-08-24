using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostSignEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[2] { "Header", "Sign" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Sign", Sign }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public HSigns Sign { get; private set; }
        #endregion

        public HostSignEventArgs(int Header, HSigns Sign)
            : base()
        {
            this.Header = Header;
            this.Sign = Sign;
        }
        public static HostSignEventArgs CreateArguments(HMessage Packet)
        {
            return new HostSignEventArgs(HHeaders.Sign = Packet.Header, (HSigns)Packet.ReadInt(0)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Sign: {1}", Header, Sign);
        }
    }
}