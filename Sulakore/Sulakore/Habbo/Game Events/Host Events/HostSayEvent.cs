using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostSayEventArgs : EventArgs, IHabboEvent
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

        public HostSayEventArgs(int Header, string Message, HThemes Theme)
            : base()
        {
            this.Header = Header;
            this.Message = Message;
            this.Theme = Theme;
        }
        public static HostSayEventArgs CreateArguments(HMessage Packet)
        {
            int P = 0;
            return new HostSayEventArgs(HHeaders.Say = Packet.Header, Packet.ReadString(ref P), (HThemes)Packet.ReadInt(P)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Message: {1} | Theme: {2}", Header, Message, Theme);
        }
    }
}