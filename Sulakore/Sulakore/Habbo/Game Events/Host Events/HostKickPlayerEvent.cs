using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostKickPlayerEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[2] { "Header", "PlayerID" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "PlayerID", PlayerID }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public int PlayerID { get; private set; }
        #endregion

        public HostKickPlayerEventArgs(int Header, int PlayerID)
            : base()
        {
            this.Header = Header;
            this.PlayerID = PlayerID;
        }
        public static HostKickPlayerEventArgs CreateArguments(HMessage Packet)
        {
            return new HostKickPlayerEventArgs(HHeaders.Kick = Packet.Header, Packet.ReadInt(0)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1}", Header, PlayerID);
        }
    }
}