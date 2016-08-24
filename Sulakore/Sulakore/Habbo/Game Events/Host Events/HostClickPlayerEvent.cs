using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostClickPlayerEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[4] { "Header", "PlayerID", "X", "Y" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "PlayerID", PlayerID },
                    { "X", Tile.X },
                    { "Y", Tile.Y }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public int PlayerID { get; private set; }
        public HPoint Tile { get; private set; }
        #endregion

        public HostClickPlayerEventArgs(int Header, int PlayerID, int X, int Y)
            : base()
        {
            this.Header = Header;
            this.PlayerID = PlayerID;
            this.Tile = new HPoint(X, Y);
        }
        public static HostClickPlayerEventArgs CreateArguments(HMessage Packet, int PlayerID)
        {
            return new HostClickPlayerEventArgs(HHeaders.Rotate = Packet.Header, PlayerID, Packet.ReadInt(0), Packet.ReadInt(4)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | PlayerID: {1} | Tile: {2}", Header, PlayerID, Tile.ToPoint().ToString());
        }
    }
}