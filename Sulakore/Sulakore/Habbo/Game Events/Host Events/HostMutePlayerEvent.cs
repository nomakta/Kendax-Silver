using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostMutePlayerEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[4] { "Header", "PlayerID", "RoomID", "Minutes" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "PlayerID", PlayerID },
                    { "RoomID", RoomID },
                    { "Minutes", Minutes }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public int PlayerID { get; private set; }
        public int RoomID { get; private set; }
        public int Minutes { get; private set; }
        #endregion

        public HostMutePlayerEventArgs(int Header, int PlayerID, int RoomID, int Minutes)
            : base()
        {
            this.Header = Header;
            this.PlayerID = PlayerID;
            this.RoomID = RoomID;
            this.Minutes = Minutes;
        }
        public static HostMutePlayerEventArgs CreateArguments(HMessage Packet)
        {
            return new HostMutePlayerEventArgs(HHeaders.Mute = Packet.Header, Packet.ReadInt(0), Packet.ReadInt(4), Packet.ReadInt(8)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1} | RoomID: {2} | Minutes: {3}", Header, PlayerID, RoomID, Minutes);
        }
    }
}