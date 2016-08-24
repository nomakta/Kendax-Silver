using System;
using System.IO;
using System.Net;
using Sulakore.Habbo;
using System.Drawing;
using System.Diagnostics;

namespace Sulakore
{
    public static class SKore
    {
        #region Private Static Fields
        private static DirectoryInfo CacheDirectory;
        private static object RandomSignLock, RandomThemeLock;
        private static Random RandomSignGenerator, RandomThemeGenerator;
        #endregion

        static SKore()
        {
            RandomSignLock = new object();
            RandomThemeLock = new object();
            RandomSignGenerator = new Random();
            RandomThemeGenerator = new Random();
            CacheDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
        }

        public const string ChromeAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 Safari/537.36";

        private static string _IPCookie;
        public static string GetIPCookie()
        {
            if (!string.IsNullOrEmpty(_IPCookie)) return _IPCookie;
            using (WebClient WC = new WebClient())
            {
                WC.Headers["User-Agent"] = ChromeAgent;
                string Body = WC.DownloadString("http://www.Habbo.com");
                if (Body.Contains("setCookie"))
                    _IPCookie = "YPF8827340282Jdskjhfiw_928937459182JAX666=" + Body.GetChilds("setCookie", '\'')[3];
                return _IPCookie;
            }
        }

        public static int GetPlayersOnline(HHotels Hotel)
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers["Cookie"] = GetIPCookie();
                WC.Headers["User-Agent"] = ChromeAgent;
                string Body = WC.DownloadString(Hotel.ToURL() + "/login_popup");
                if (Body.Contains("stats-fig"))
                    return int.Parse(Body.GetChild("<span class=\"stats-fig\">", '<'));
                return -1;
            }
        }
        public static int GetPlayerID(string PlayerName, HHotels Hotel)
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers["Cookie"] = GetIPCookie();
                WC.Headers["User-Agent"] = ChromeAgent;
                string Body = WC.DownloadString(Hotel.ToURL() + "/habblet/ajax/new_habboid?habboIdName=" + PlayerName);
                if (!Body.Contains("rounded rounded-red"))
                    return int.Parse(Body.GetChild("<em>", '<').Replace(" ", string.Empty));
                return -1;
            }
        }
        public static string GetPlayerName(int PlayerID, HHotels Hotel)
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers["Cookie"] = GetIPCookie();
                WC.Headers["User-Agent"] = ChromeAgent;
                string Body = WC.DownloadString(string.Format("{0}/rd/{1}", Hotel.ToURL(), PlayerID));
                if (Body.Contains("/home/"))
                    return Body.GetChild("<input type=\"hidden\" name=\"page\" value=\"/home/", '?');
                return string.Empty;
            }
        }
        public static bool CheckPlayerName(string PlayerName, HHotels Hotel)
        {
            return GetPlayerID(PlayerName, Hotel) == -1;
        }
        public static string GetPlayerMotto(string PlayerName, HHotels Hotel)
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers["Cookie"] = GetIPCookie();
                WC.Headers["User-Agent"] = ChromeAgent;
                string Body = WC.DownloadString(Hotel.ToURL() + "/habblet/habbosearchcontent?searchString=" + PlayerName);
                if (Body.IndexOf(PlayerName, StringComparison.OrdinalIgnoreCase) != -1)
                    return Body.GetChild("<b>" + PlayerName + "</b><br />", '<');
                return string.Empty;
            }
        }
        public static Bitmap GetPlayerAvatar(string PlayerName, HHotels Hotel)
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers["Cookie"] = GetIPCookie();
                WC.Headers["User-Agent"] = ChromeAgent;
                byte[] AvatarData = WC.DownloadData(Hotel.ToURL() + "/habbo-imaging/avatarimage?user=" + PlayerName + "&action=&direction=&head_direction=&gesture=&size=");
                using (MemoryStream MS = new MemoryStream(AvatarData))
                    return new Bitmap(MS);
            }
        }
        public static string GetPlayerFigure(string PlayerName, HHotels Hotel)
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers["Cookie"] = GetIPCookie();
                WC.Headers["User-Agent"] = ChromeAgent;
                string Body = WC.DownloadString(Hotel.ToURL() + "/habblet/habbosearchcontent?searchString=" + PlayerName);
                return string.Empty;
            }
        }
        public static string GetPlayerLastOnline(string PlayerName, HHotels Hotel, bool Exact = true)
        {
            using (WebClient WC = new WebClient())
            {
                WC.Headers["Cookie"] = GetIPCookie();
                WC.Headers["User-Agent"] = ChromeAgent;
                string Body = WC.DownloadString(Hotel.ToURL() + "/habblet/habbosearchcontent?searchString=" + PlayerName);
                if (Body.Contains("lastlogin"))
                {
                    Body = Body.GetChild("<div class=\"lastlogin\">").GetChild("span title=");
                    if (Exact) return Body.Split('"')[1];
                    else return Body.Split('>')[1].Split('<')[0];
                }
                return string.Empty;
            }
        }

        public static void ClearCache(bool WinINet = false)
        {
            FileInfo[] CacheFiles = CacheDirectory.GetFiles();
            foreach (FileInfo CacheFile in CacheFiles)
                try { CacheFile.Delete(); }
                catch { continue; }

            DirectoryInfo[] CacheDirectories = CacheDirectory.GetDirectories();
            foreach (DirectoryInfo CacheFolder in CacheDirectories)
                try { CacheFolder.Delete(true); }
                catch { continue; }

            if (WinINet)
                try { (Process.Start("RunDll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 10")).Dispose(); }
                catch { }
        }
        public static void Unsubscribe<T>(ref EventHandler<T> Event) where T : EventArgs
        {
            if (Event == null) return;
            Delegate[] Subscriptions = Event.GetInvocationList();
            foreach (Delegate Subscription in Subscriptions)
                Event -= (EventHandler<T>)Subscription;
        }

        public static int Juice(this HSigns Sign)
        {
            if (Sign != HSigns.Random) return (int)Sign;
            else
            {
                lock (RandomSignLock)
                    return RandomSignGenerator.Next(0, 19);
            }
        }
        public static string Juice(this HBans Ban)
        {
            switch (Ban)
            {
                case HBans.Day: return "RWUAM_BAN_USER_DAY";
                case HBans.Hour: return "RWUAM_BAN_USER_HOUR";
                case HBans.Permanent: return "RWUAM_BAN_USER_PERM";
                default: return "RWUAM_BAN_USER_DAY";
            }
        }
        public static int Juice(this HThemes Theme)
        {
            if (Theme != HThemes.Random) return (int)Theme;
            else
            {
                lock (RandomThemeLock)
                    return RandomThemeGenerator.Next(0, 30);
            }
        }
        public static string Juice(this HPages Page, HHotels Hotel)
        {
            switch (Page)
            {
                case HPages.Client: return Hotel.ToURL() + "/client";
                case HPages.Home: return Hotel.ToURL() + "/home/";
                case HPages.IDAvatars: return Hotel.ToURL() + "/identity/avatars";
                case HPages.IDSettings: return Hotel.ToURL() + "/identity/settings";
                case HPages.Me: return Hotel.ToURL() + "/me";
                case HPages.Profile: return Hotel.ToURL() + "/profile";
                default: return string.Empty;
            }
        }

        public static HHotels Convert(string Host)
        {
            return (HHotels)Enum.Parse(typeof(HHotels), Host.GetChild("www.habbo.").Replace(".", "_"));
        }

        public static string ToDomain(this HHotels Hotel)
        {
            return Hotel.ToString().Replace('_', '.');
        }
        public static string ToURL(this HHotels Hotel, bool HTTPS = false)
        {
            return (HTTPS ? "https://www.Habbo." : "http://www.Habbo.") + Hotel.ToDomain();
        }

        public static string GetChild(this string Body, string Parent)
        {
            return Body.Substring(Body.IndexOf(Parent, StringComparison.OrdinalIgnoreCase) + Parent.Length).Trim();
        }
        public static string GetChild(this string Body, string Parent, char Delimiter)
        {
            return GetChilds(Body, Parent, Delimiter)[0].Trim();
        }
        public static string[] GetChilds(this string Body, string Parent, char Delimiter)
        {
            return GetChild(Body, Parent).Split(Delimiter);
        }
    }
}