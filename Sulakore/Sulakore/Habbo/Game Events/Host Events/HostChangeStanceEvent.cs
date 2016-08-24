using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostChangeStanceEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[2] { "Header", "Stance" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Stance", Stance }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public HStances Stance { get; private set; }
        #endregion

        public HostChangeStanceEventArgs(int Header, HStances Stance)
            : base()
        {
            this.Header = Header;
            this.Stance = Stance;
        }
        public static HostChangeStanceEventArgs CreateArguments(HMessage Packet)
        {
            return new HostChangeStanceEventArgs(HHeaders.Stance = Packet.Header, (HStances)Packet.ReadInt(0)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Stance: {1}", Header, Stance);
        }
    }
}