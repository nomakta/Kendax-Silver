using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Sulakore.Protocol;

namespace Sulakore.Habbo
{
    public class HostTradeEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[2] { "Header", "Index" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "Header", Header },
                    { "Index", Index }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public int Index { get; private set; }
        #endregion

        public HostTradeEventArgs(int Header, int Index)
            : base()
        {
            this.Header = Header;
            this.Index = Index;
        }
        public static HostTradeEventArgs CreateArguments(HMessage Packet)
        {
            return new HostTradeEventArgs(HHeaders.Trade = Packet.Header, Packet.ReadInt(0)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Index: {1}", Header, Index);
        }
    }
}