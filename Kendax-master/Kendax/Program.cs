using System;
using Sulakore;
using System.Linq;
using Sulakore.Habbo;
using System.Windows.Forms;
using System.Collections.Generic;
using Sulakore.Protocol.Encryption;

namespace Kendax
{
    static class Program
    {
        public static Dictionary<string, HSession> Emails { get; set; }
        public static Dictionary<string, HSession> Accounts { get; set; }
        public static Dictionary<HSession, HKeyExchange> Connections { get; set; }

        [STAThread]
        static void Main()
        {
            LoadHeaders();
            Emails = new Dictionary<string, HSession>();
            Accounts = new Dictionary<string, HSession>();
            Connections = new Dictionary<HSession, HKeyExchange>();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        static void LoadHeaders()
        {
            HHeaders.Motto = 1163;
            HHeaders.Say = 930;
            HHeaders.Walk = 1998;
            HHeaders.Pong = 2505;
            HHeaders.Dance = 1017;
            HHeaders.Sign = 3939;
            HHeaders.Shout = 2431;
            HHeaders.Rotate = 3390;
            HHeaders.Stance = 991;
            HHeaders.Roomlike = 1044;
            HHeaders.Trade = 2419;
            HHeaders.Respect = 2168;
            HHeaders.AddFriend = 2977;
            HHeaders.Clothes = 1769;
            HHeaders.Gesture = 2460;
            HHeaders.Navigate = 1820;
            HHeaders.Exit = 821;
            HHeaders.Groupjoin = 1150;
            HHeaders.HelpRequest = 1943;
            HHeaders.Scratch = 651;
            HHeaders.RideHorse = 479;
            HHeaders.MakeRoom = 1844;

        }
    }
}