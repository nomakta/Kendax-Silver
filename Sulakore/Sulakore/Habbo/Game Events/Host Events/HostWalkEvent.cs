using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostWalkEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[3] { "Header", "X", "Y" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "X", Tile.X },
                    { "Y", Tile.Y }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public HPoint Tile { get; private set; }
        #endregion

        public HostWalkEventArgs(int Header, int X, int Y)
            : base()
        {
            this.Header = Header;
            this.Tile = new HPoint(X, Y);
        }
        public static HostWalkEventArgs CreateArguments(HMessage Packet)
        {
            return new HostWalkEventArgs(HHeaders.Walk = Packet.Header, Packet.ReadInt(0), Packet.ReadInt(4)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Tile: {1}", Header, Tile.ToPoint().ToString());
        }
    }
}