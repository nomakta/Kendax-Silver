using System;
using Sulakore.Protocol;
using Sulakore.Communication;
using Sulakore.Protocol.Encryption;

namespace Sulakore.Habbo
{
    public abstract class HBase : IHServer, IHClient
    {
        public abstract event EventHandler<EventArgs> OnConnected;
        public abstract event EventHandler<EventArgs> OnDisconnected;
        public abstract event EventHandler<DataToEventArgs> DataToClient;
        public abstract event EventHandler<DataToEventArgs> DataToServer;

        #region Game Events
        public event EventHandler<HostSayEventArgs> OnHostSay;
        public event EventHandler<HostExitEventArgs> OnHostExit;
        public event EventHandler<HostWalkEventArgs> OnHostWalk;
        public event EventHandler<HostSignEventArgs> OnHostSign;
        public event EventHandler<HostDanceEventArgs> OnHostDance;
        public event EventHandler<HostShoutEventArgs> OnHostShout;
        public event EventHandler<HostTradeEventArgs> OnHostTrade;
        public event EventHandler<HostKickedEventArgs> OnHostKicked;
        public event EventHandler<HostGestureEventArgs> OnHostGesture;
        public event EventHandler<HostNavigateEventArgs> OnHostNavigate;
        public event EventHandler<HostBanPlayerEventArgs> OnHostBanPlayer;
        public event EventHandler<HostMutePlayerEventArgs> OnHostMutePlayer;
        public event EventHandler<HostKickPlayerEventArgs> OnHostKickPlayer;
        public event EventHandler<HostClickPlayerEventArgs> OnHostClickPlayer;
        public event EventHandler<HostChangeMottoEventArgs> OnHostChangeMotto;
        public event EventHandler<HostChangeStanceEventArgs> OnHostChangeStance;
        public event EventHandler<HostMoveFurnitureEventArgs> OnHostMoveFurniture;
        public event EventHandler<HostChangeClothesEventArgs> OnHostChangeClothes;

        //public event EventHandler<PlayerSayEventArgs> OnPlayerSay;
        //public event EventHandler<PlayerWalkEventArgs> OnPlayerWalk;
        //public event EventHandler<PlayerSignEventArgs> OnPlayerSign;
        //public event EventHandler<PlayerShoutEventArgs> OnPlayerShout;
        //public event EventHandler<PlayerEnterEventArgs> OnPlayerEnter;
        //public event EventHandler<PlayerDanceEventArgs> OnPlayerDance;
        //public event EventHandler<PlayerGestureEventArgs> OnPlayerGesture;
        //public event EventHandler<PlayerChangeDataEventArgs> OnPlayerChangeData;
        //public event EventHandler<PlayerChangeStanceEventArgs> OnPlayerChangeStance;
        //public event EventHandler<PlayerMoveFurnitureEventArgs> OnPlayerMoveFurniture;
        //public event EventHandler<PlayerDropFurnitureEventArgs> OnPlayerDropFurniture;
        #endregion

        public abstract int Port { get; protected set; }
        public abstract string Host { get; protected set; }
        public abstract string[] Addresses { get; protected set; }
        public abstract bool RequestEncrypted { get; protected set; }
        public abstract bool ResponseEncrypted { get; protected set; }

        public abstract bool IsConnected { get; }
        public abstract bool CaptureEvents { get; set; }

        public abstract HRC4 ServerEncrypt { get; set; }
        public abstract HRC4 ServerDecrypt { get; set; }

        public abstract HRC4 ClientEncrypt { get; set; }
        public abstract HRC4 ClientDecrypt { get; set; }

        public abstract int SendToClient(byte[] Data);
        public abstract int SendToServer(byte[] Data);

        protected void DataFromClient(byte[] Data)
        {
            HMessage Packet = new HMessage(Data);
        }
        protected void DataFromServer(byte[] Data)
        {
            HMessage Packet = new HMessage(Data);
        }
    }
}