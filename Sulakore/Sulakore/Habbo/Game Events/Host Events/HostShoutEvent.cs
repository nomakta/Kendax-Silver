using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostShoutEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[3] { "Header", "Message", "Theme" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Message", Message },
                    { "Theme", Theme }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public string Message { get; private set; }
        public HThemes Theme { get; private set; }
        #endregion

        public HostShoutEventArgs(int Header, string Message, HThemes Theme)
            : base()
        {
            this.Header = Header;
            this.Message = Message;
            this.Theme = Theme;
        }
        public static HostShoutEventArgs CreateArguments(HMessage Packet)
        {
            return new HostShoutEventArgs(HHeaders.Shout = Packet.Header, Packet.ReadString(0), (HThemes)Packet.ReadInt(Packet.Length - 6)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Message: {1} | Theme: {2}", Header, Message, Theme);
        }
    }
}