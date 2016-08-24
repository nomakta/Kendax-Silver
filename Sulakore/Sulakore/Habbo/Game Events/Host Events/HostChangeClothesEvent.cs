using Sulakore.Protocol;
using System;
using System.Collections.Generic;

namespace Sulakore.Habbo
{
    public class HostChangeClothesEventArgs : EventArgs, IHabboEvent
    {
        #region Properties
        public static object[] Params
        {
            get { return new object[3] { "Header", "Gender", "Clothes" }; }
        }
        public Dictionary<string, object> Data
        {
            get
            {
                return new Dictionary<string, object> 
                {
                    { "Header", Header },
                    { "Gender", Gender },
                    { "Clothes", Clothes }
                };
            }
        }
        public HMessage Packet { get; private set; }

        public int Header { get; private set; }
        public HGenders Gender { get; private set; }
        public string Clothes { get; private set; }
        #endregion

        public HostChangeClothesEventArgs(int Header, HGenders Gender, string Clothes)
            : base()
        {
            this.Header = Header;
            this.Gender = Gender;
            this.Clothes = Clothes;
        }
        public static HostChangeClothesEventArgs CreateArguments(HMessage Packet)
        {
            return new HostChangeClothesEventArgs(HHeaders.Clothes = Packet.Header, (HGenders)Packet.ReadString(0).ToUpper()[0], Packet.ReadString(3)) { Packet = new HMessage(Packet.ToBytes(), HDestinations.Server) };
        }

        public override string ToString()
        {
            return string.Format("Header: {0} | Gender: {1} | Clothes: {2}", Header, Gender, Clothes);
        }
    }
}