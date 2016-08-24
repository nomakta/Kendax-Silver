using System;
using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostBanPlayerEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[4] { "Header", "PlayerID", "RoomID", "Ban" }; }
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
                    { "Ban", Ban }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public int PlayerID { get; private set; }
        public int RoomID { get; private set; }
        public HBans Ban { get; private set; }
        #endregion

        public HostBanPlayerEventArgs(int Header, int PlayerID, int RoomID, HBans Ban)
            : base()
        {
            this.Header = Header;
            this.PlayerID = PlayerID;
            this.RoomID = RoomID;
            this.Ban = Ban;
        }
        public static HostBanPlayerEventArgs CreateArguments(HMessage Packet)
        {
            return new HostBanPlayerEventArgs(HHeaders.Ban = Packet.Header, Packet.ReadInt(0), Packet.ReadInt(4), (HBans)Enum.Parse(typeof(HBans), Packet.ReadString(8), true)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1} | RoomID: {2} | Ban: {3}", Header, PlayerID, RoomID, Ban);
        }
    }
}