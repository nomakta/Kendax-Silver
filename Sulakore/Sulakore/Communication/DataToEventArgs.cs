using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class DataToEventArgs : EventArgs
    {
        public bool Skip { get; set; }
        public HMessage Packet { get; set; }
        public int Step { get; private set; }

        public DataToEventArgs(byte[] Data)
            : base()
        {
            Packet = new HMessage(Data);
        }

        public DataToEventArgs(byte[] Data, HDestinations Destination, int Step)
            : base()
        {
            this.Step = Step;
            Packet = new HMessage(Data, Destination);
        }
    }
}