namespace Sulakore.Habbo
{
    public struct HHeaders
    {
        #region Properties
        public static ushort Say { get; set; }
        public static ushort Shout { get; set; }

        public static ushort Kick { get; set; }
        public static ushort Mute { get; set; }
        public static ushort Ban { get; set; }
        public static ushort Groupjoin { get; set; }
        public static ushort HelpRequest { get; set; }
        public static ushort Scratch { get; set; }
        public static ushort RideHorse { get; set; }
        public static ushort MakeRoom { get; set; }
        public static ushort Roomlike { get; set; }
        public static ushort Pong { get; set; }
        public static ushort Exit { get; set; }
        public static ushort Walk { get; set; }
        public static ushort Sign { get; set; }
        public static ushort Dance { get; set; }
        public static ushort Motto { get; set; }
        public static ushort Trade { get; set; }
        public static ushort Rotate { get; set; }
        public static ushort Stance { get; set; }
        public static ushort Clothes { get; set; }
        public static ushort Gesture { get; set; }
        public static ushort Respect { get; set; }
        public static ushort AddFriend { get; set; }

        public static ushort Navigate { get; set; }
        public static ushort PlayerSay { get; set; }
        public static ushort RoomLoaded { get; set; }
        public static ushort PlayerShout { get; set; }
        public static ushort PlayerDance { get; set; }
        public static ushort MoveFurniture { get; set; }
        public static ushort PlayerGesture { get; set; }
        #endregion

        public static void Reset()

        {
            Say = 0;
            Ban = 0;
            Pong = 0;
            Exit = 0;
            Walk = 0;
            Kick = 0;
            Mute = 0;
            Sign = 0;
            Shout = 0;
            Dance = 0;
            Motto = 0;
            Trade = 0;
            Rotate = 0;
            Stance = 0;
            Clothes = 0;
            Gesture = 0;
            Respect = 0;
            AddFriend = 0;
            Navigate = 0;
            PlayerSay = 0;
            RoomLoaded = 0;
            PlayerShout = 0;
            PlayerDance = 0;
            MoveFurniture = 0;
            PlayerGesture = 0;
            Groupjoin = 0;
            Roomlike = 0;
            MakeRoom = 0;
            RideHorse = 0;
            Scratch = 0;
            HelpRequest = 0;

        }
    }
}