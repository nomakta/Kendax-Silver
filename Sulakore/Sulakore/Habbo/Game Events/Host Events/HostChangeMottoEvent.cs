using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostChangeMottoEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[2] { "Header", "Motto" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Motto", Motto }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public string Motto { get; private set; }
        #endregion

        public HostChangeMottoEventArgs(int Header, string Motto)
            : base()
        {
            this.Header = Header;
            this.Motto = Motto;
        }
        public static HostChangeMottoEventArgs CreateArguments(HMessage Packet)
        {
            return new HostChangeMottoEventArgs(HHeaders.Motto = Packet.Header, Packet.ReadString(0)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Motto: {1}", Header, Motto);
        }
    }
}