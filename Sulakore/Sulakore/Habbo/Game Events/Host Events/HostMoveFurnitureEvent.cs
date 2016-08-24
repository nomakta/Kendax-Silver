using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostMoveFurnitureEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[6] { "Header", "FurniID", "X", "Y", "Z", "Direction" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "FurniID", FurniID },
                    { "X", Tile.X },
                    { "Y", Tile.Y },
                    { "Z", Tile.Z },
                    { "Direction", Direction }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public int FurniID { get; private set; }
        public HPoint Tile { get; private set; }
        public HDirections Direction { get; private set; }
        #endregion

        public HostMoveFurnitureEventArgs(int Header, int FurniID, int X, int Y, string Z, HDirections Direction)
            : base()
        {
            this.Header = Header;
            this.FurniID = FurniID;
            this.Tile = new HPoint(X, Y, Z);
            this.Direction = Direction;
        }
        public static HostMoveFurnitureEventArgs CreateArguments(HMessage Packet, string Z)
        {
            return new HostMoveFurnitureEventArgs(HHeaders.MoveFurniture = Packet.Header, Packet.ReadInt(0), Packet.ReadInt(4), Packet.ReadInt(8), Z, (HDirections)Packet.ReadInt(12)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | FurniID: {1} | Tile: {2} | Direction: {3}", Header, FurniID, Tile.ToString(), Direction);
        }
    }
}