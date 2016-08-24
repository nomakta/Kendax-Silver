using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostNavigateEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[3] { "Header", "RoomID", "Password" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "RoomID", RoomID },
                    { "Password", Password }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public int RoomID { get; private set; }
        public string Password { get; private set; }
        #endregion

        public HostNavigateEventArgs(int Header, int RoomID, string Password)
            : base()
        {
            this.Header = Header;
            this.RoomID = RoomID;
            this.Password = Password;
        }
        public static HostNavigateEventArgs CreateArguments(HMessage Packet)
        {
            return new HostNavigateEventArgs(HHeaders.Navigate = Packet.Header, Packet.ReadInt(0), Packet.ReadString(4)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | RoomID: {1} | Password: {2}", Header, RoomID, Password);
        }
    }
}