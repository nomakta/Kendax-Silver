using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostGestureEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[2] { "Header", "Gesture" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Gesture", Gesture }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public HGestures Gesture { get; private set; }
        #endregion

        public HostGestureEventArgs(int Header, HGestures Gesture)
            : base()
        {
            this.Header = Header;
            this.Gesture = Gesture;
        }
        public static HostGestureEventArgs CreateArguments(HMessage Packet)
        {
            return new HostGestureEventArgs(HHeaders.Gesture = Packet.Header, (HGestures)Packet.ReadInt(0)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Gesture: {1}", Header, Gesture);
        }
    }
}