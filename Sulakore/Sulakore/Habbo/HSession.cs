using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Sulakore.Wrappers;
using Sulakore.Protocol;
using System.Net.Sockets;
using System.Net.Security;
using Sulakore.Communication;
using System.Collections.Generic;
using Sulakore.Protocol.Encryption;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;

namespace Sulakore.Habbo
{
    public sealed class HSession : IPlayerSession, IDisposable
    {
        #region Connection Events
        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisconnected;
        public event EventHandler<DataToEventArgs> DataToServer;
        public event EventHandler<DataToEventArgs> DataToClient;
        #endregion

        #region Private Fields
        private Socket ServerS;
        private object DisconnectLock;
        private byte[] ServerB, ServerC;
        private int ToClientS, ToServerS;

        private LoginCallback _Login;
        private AddFriendCallback _AddFriend;
        private RemoveFriendCallback _RemoveFriend;
        private UpdateProfileCallback _UpdateProfile;

        private Dictionary<IAsyncResult, LoginCallback> LoginCallbacks;
        private Dictionary<IAsyncResult, AddFriendCallback> AddFriendCallbacks;
        private Dictionary<IAsyncResult, RemoveFriendCallback> RemoveFriendCallbacks;
        private Dictionary<IAsyncResult, UpdateProfileCallback> UpdateProfileCallbacks;

        private delegate bool LoginCallback();
        private delegate void AddFriendCallback(int PlayerID);
        private delegate void RemoveFriendCallback(int PlayerID);
        private delegate void UpdateProfileCallback(string Motto, bool HomepageVisible, bool FriendRequestAllowed, bool ShowOnlineStatus, bool OfflineMessaging, bool FriendsCanFollow);
        #endregion

        #region Public Properties
        public bool IsLoggedIn
        {
            get
            {
                using (WebClientEx WC = new WebClientEx(Cookies))
                {
                    WC.Headers["User-Agent"] = SKore.ChromeAgent;
                    string Body = WC.DownloadString(Hotel.ToURL(true));
                    return Body.Contains("window.location.replace('http:\\/\\/www.habbo." + Hotel.ToDomain() + "\\/me')");
                }
            }
        }

        private string _URLToken;
        public string URLToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_URLToken)) return _URLToken;
                LoadResource(HPages.Profile);
                return _URLToken;
            }
        }

        private string _CSRFToken;
        public string CSRFToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_CSRFToken)) return _CSRFToken;
                LoadResource(HPages.Profile);
                return _URLToken;
            }
        }

        private string _PlayerName;
        public string PlayerName
        {
            get
            {
                if (!string.IsNullOrEmpty(_PlayerName)) return _PlayerName;
                LoadResource(HPages.Profile);
                return _PlayerName;
            }
        }

        private string _Motto;
        public string Motto
        {
            get
            {
                if (!string.IsNullOrEmpty(_Motto)) return _Motto;
                LoadResource(HPages.Me);
                return _Motto;
            }
        }

        private string _LastSignIn;
        public string LastSignIn
        {
            get
            {
                if (!string.IsNullOrEmpty(_LastSignIn)) return _LastSignIn;
                LoadResource(HPages.Me);
                return _LastSignIn;
            }
        }

        private string _CreatedOn;
        public string CreatedOn
        {
            get
            {
                if (!string.IsNullOrEmpty(_CreatedOn)) return _CreatedOn;
                LoadResource(HPages.Home);
                return _CreatedOn;
            }
        }

        private HGenders _Gender;
        public HGenders Gender
        {
            get
            {
                if (_Gender != HGenders.Unknown) return _Gender;
                LoadResource(HPages.Profile);
                return _Gender;
            }
        }

        private int _Age;
        public int Age
        {
            get
            {
                if (_Age != 0) return _Age;
                LoadResource(HPages.Profile);
                return _Age;
            }
        }

        private string _UserHash;
        public string UserHash
        {
            get
            {
                if (!string.IsNullOrEmpty(_UserHash)) return _UserHash;
                LoadResource(HPages.Client);
                return _UserHash;
            }
        }

        public string Email { get; private set; }
        public string Password { get; private set; }
        public HHotels Hotel { get; private set; }

        public int PlayerID { get; private set; }
        public string ClientStarting { get; set; }
        public CookieContainer Cookies { get; private set; }

        HRC4 IHClient.ServerEncrypt { get; set; }
        public HRC4 ServerDecrypt { get; set; }

        public HRC4 ClientEncrypt { get; set; }
        HRC4 IHServer.ClientDecrypt { get; set; }

        private HGameData _GameData;
        public HGameData GameData
        {
            get
            {
                if (!_GameData.IsEmpty) return _GameData;
                LoadResource(HPages.Client);
                return _GameData;
            }
        }

        private string _FlashClientURL;
        public string FlashClientURL
        {
            get
            {
                if (!string.IsNullOrEmpty(_FlashClientURL)) return _FlashClientURL;
                LoadResource(HPages.Client);
                return _FlashClientURL;
            }
        }

        private string _FlashClientRevision;
        public string FlashClientRevision
        {
            get
            {
                if (!string.IsNullOrEmpty(_FlashClientRevision)) return _FlashClientRevision;
                LoadResource(HPages.Client);
                return _FlashClientRevision;
            }
        }

        private int _Port;
        public int Port
        {
            get
            {
                if (_Port != 0) return _Port;
                LoadResource(HPages.Client);
                return _Port;
            }
        }

        private string _Host;
        public string Host
        {
            get
            {
                if (!string.IsNullOrEmpty(_Host)) return _Host;
                LoadResource(HPages.Client);
                return _Host;
            }
        }

        private string[] _Addresses;
        public string[] Addresses
        {
            get
            {
                if (_Addresses != null && _Addresses.Length > 0) return _Addresses;
                LoadResource(HPages.Client);
                return _Addresses;
            }
        }

        private string _SSOTicket;
        public string SSOTicket
        {
            get
            {
                if (!string.IsNullOrEmpty(_SSOTicket)) return _SSOTicket;
                LoadResource(HPages.Client);
                return _SSOTicket;
            }
        }

        private bool _ReceiveData;
        public bool ReceiveData
        {
            get { return _ReceiveData; }
            set
            {
                if (!IsConnected) _ReceiveData = false;
                else if (_ReceiveData != value)
                {
                    bool WasReceiving = _ReceiveData;
                    _ReceiveData = value;
                    if (!WasReceiving) ReadServerData();
                }
            }
        }

        public bool IsConnected
        {
            get { return ServerS != null && ServerS.Connected; }
        }
        public bool RequestEncrypted { get; private set; }
        public bool ResponseEncrypted { get; private set; }
        #endregion

        #region Constructor(s)
        static HSession()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.ServerCertificateValidationCallback = ValidateCertificate;
        }
        public HSession(string Cookies, HHotels Hotel)
        {
            this.Hotel = Hotel;

            DisconnectLock = new object();

            this.Cookies = new CookieContainer();
            this.Cookies.SetCookies(new Uri(Hotel.ToURL()), Cookies);

            _Login = Login;
            _AddFriend = AddFriend;
            _RemoveFriend = RemoveFriend;
            _UpdateProfile = UpdateProfile;

            LoginCallbacks = new Dictionary<IAsyncResult, LoginCallback>();
            AddFriendCallbacks = new Dictionary<IAsyncResult, AddFriendCallback>();
            RemoveFriendCallbacks = new Dictionary<IAsyncResult, RemoveFriendCallback>();
            UpdateProfileCallbacks = new Dictionary<IAsyncResult, UpdateProfileCallback>();
        }
        public HSession(string Email, string Password, HHotels Hotel)
            : this(SKore.GetIPCookie(), Hotel)
        {
            this.Email = Email;
            this.Password = Password;
        }
        #endregion

        #region Indexer(s)
        public string this[HPages Page]
        {
            get { return LoadResource(Page); }
        }
        #endregion

        #region Public Methods
        public bool Login()
        {
            try
            {
                #region Authentication
                byte[] PostData = Encoding.Default.GetBytes(string.Format("credentials.username={0}&credentials.password={1}", Email, Password));
                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/account/submit", Hotel.ToURL(true)));
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = SKore.ChromeAgent;
                Request.AllowAutoRedirect = false;
                Request.CookieContainer = Cookies;
                Request.Method = "POST";

                using (Stream DataStream = Request.GetRequestStream())
                    DataStream.Write(PostData, 0, PostData.Length);

                string Body = string.Empty;
                using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
                {
                    Cookies.Add(Response.Cookies);
                    using (StreamReader SR = new StreamReader(Response.GetResponseStream()))
                        Body = SR.ReadToEnd();
                }
                #endregion

                if (Body.Contains("useOrCreateAvatar"))
                {
                    #region Player Selection
                    PlayerID = int.Parse(Body.GetChild("useOrCreateAvatar/", '?'));
                    Request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/identity/useOrCreateAvatar/{1}?next=", Hotel.ToURL(), PlayerID));
                    Request.UserAgent = SKore.ChromeAgent;
                    Request.CookieContainer = Cookies;
                    Request.AllowAutoRedirect = false;
                    Request.Method = "GET";

                    string RedirectingTo = string.Empty;
                    using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
                    {
                        Cookies.Add(Response.Cookies);
                        RedirectingTo = Response.Headers["Location"];
                    }
                    #endregion

                    #region Manual Redirect
                    Request = (HttpWebRequest)WebRequest.Create(RedirectingTo);
                    Request.UserAgent = SKore.ChromeAgent;
                    Request.CookieContainer = Cookies;
                    Request.AllowAutoRedirect = false;
                    Request.Method = "GET";

                    using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
                    {
                        Cookies.Add(Response.Cookies);

                        using (StreamReader SR = new StreamReader(Response.GetResponseStream()))
                            Body = SR.ReadToEnd();

                        if (RedirectingTo.EndsWith("/me"))
                        {
                            HandleResource(HPages.Me, ref Body);
                            return true;
                        }
                    }
                    #endregion

                    if (Body.Contains("/account/updateIdentityProfileTerms"))
                    {
                        #region Accept Terms Of Service
                        PostData = Encoding.Default.GetBytes("termsSelection=true");
                        Request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/account/updateIdentityProfileTerms", Hotel.ToURL(true)));
                        Request.ContentType = "application/x-www-form-urlencoded";
                        Request.Headers["Origin"] = Hotel.ToURL(true);
                        Request.UserAgent = SKore.ChromeAgent;
                        Request.AllowAutoRedirect = false;
                        Request.CookieContainer = Cookies;
                        Request.Referer = RedirectingTo;
                        Request.Method = "POST";

                        using (Stream DataStream = Request.GetRequestStream())
                            DataStream.Write(PostData, 0, PostData.Length);

                        using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
                            Cookies.Add(Response.Cookies);
                        #endregion
                    }
                    else if (Body.Contains("/account/updateIdentityProfileEmail"))
                    {
                        #region Skip Email Verification
                        PostData = Encoding.Default.GetBytes("email=&skipEmailChange=true");
                        Request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/account/updateIdentityProfileEmail", Hotel.ToURL(true)));
                        Request.ContentType = "application/x-www-form-urlencoded";
                        Request.Headers["Origin"] = Hotel.ToURL(true);
                        Request.UserAgent = SKore.ChromeAgent;
                        Request.AllowAutoRedirect = false;
                        Request.CookieContainer = Cookies;
                        Request.Referer = RedirectingTo;
                        Request.Method = "POST";

                        using (Stream DataStream = Request.GetRequestStream())
                            DataStream.Write(PostData, 0, PostData.Length);

                        using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
                            Cookies.Add(Response.Cookies);
                        #endregion
                    }

                    if (Body.Contains("/account/updateIdentityProfileTerms") || Body.Contains("/account/updateIdentityProfileEmail"))
                    {
                        #region Player Selection
                        Request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/identity/useOrCreateAvatar/{1}?disableFriendLinking=false&combineIdentitiesSelection=2&next=&selectedAvatarId=", Hotel.ToURL(), PlayerID));
                        Request.UserAgent = SKore.ChromeAgent;
                        Request.CookieContainer = Cookies;
                        Request.AllowAutoRedirect = false;
                        Request.Method = "GET";

                        using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
                        {
                            Cookies.Add(Response.Cookies);
                            RedirectingTo = Response.Headers["Location"];
                        }
                        #endregion

                        #region Manual Redirect
                        Request = (HttpWebRequest)WebRequest.Create(RedirectingTo);
                        Request.UserAgent = SKore.ChromeAgent;
                        Request.CookieContainer = Cookies;
                        Request.AllowAutoRedirect = false;
                        Request.Method = "GET";

                        using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
                        {
                            Cookies.Add(Response.Cookies);

                            using (StreamReader SR = new StreamReader(Response.GetResponseStream()))
                                Body = SR.ReadToEnd();

                            if (RedirectingTo.EndsWith("/me"))
                            {
                                HandleResource(HPages.Me, ref Body);
                                return true;
                            }
                        }
                        #endregion
                    }
                }
                return false;
            }
            catch { return false; }
        }
        public IAsyncResult BeginLogin(AsyncCallback callback, object state)
        {
            IAsyncResult iAr = _Login.BeginInvoke(callback, state);
            LoginCallbacks.Add(iAr, _Login);
            return iAr;
        }
        public bool EndLogin(IAsyncResult iAr)
        {
            bool Value = false;
            if (LoginCallbacks.ContainsKey(iAr))
            {
                Value = LoginCallbacks[iAr].EndInvoke(iAr);
                LoginCallbacks.Remove(iAr);
            }
            return Value;
        }

        public void AddFriend(int PlayerID)
        {
            using (WebClientEx WC = new WebClientEx(Cookies))
            {
                NameValueCollection Arguments = new NameValueCollection(1);
                Arguments.Add("accountId", PlayerID.ToString());

                WC.Headers["X-App-Key"] = CSRFToken;
                WC.Headers["User-Agent"] = SKore.ChromeAgent;
                WC.UploadValues(Hotel.ToURL() + "/myhabbo/friends/add", Arguments);
            }
        }
        public IAsyncResult BeginAddFriend(int PlayerID, AsyncCallback callback, object state)
        {
            IAsyncResult iAr = _AddFriend.BeginInvoke(PlayerID, callback, state);
            AddFriendCallbacks.Add(iAr, _AddFriend);
            return iAr;
        }
        public void EndAddFriend(IAsyncResult iAr)
        {
            if (AddFriendCallbacks.ContainsKey(iAr))
            {
                AddFriendCallbacks[iAr].EndInvoke(iAr);
                AddFriendCallbacks.Remove(iAr);
            }
        }

        public void RemoveFriend(int PlayerID)
        {
            using (WebClientEx WC = new WebClientEx(Cookies))
            {
                NameValueCollection Arguments = new NameValueCollection(2);
                Arguments.Add("friendId", PlayerID.ToString());
                Arguments.Add("pageSize", "30");

                WC.Headers["X-App-Key"] = CSRFToken;
                WC.Headers["User-Agent"] = SKore.ChromeAgent;
                WC.UploadValues(Hotel.ToURL(true) + "/friendmanagement/ajax/deletefriends", Arguments);
            }
        }
        public IAsyncResult BeginRemoveFriend(int PlayerID, AsyncCallback callback, object state)
        {
            IAsyncResult iAr = _RemoveFriend.BeginInvoke(PlayerID, callback, state);
            RemoveFriendCallbacks.Add(iAr, _RemoveFriend);
            return iAr;
        }
        public void EndRemoveFriend(IAsyncResult iAr)
        {
            if (RemoveFriendCallbacks.ContainsKey(iAr))
            {
                RemoveFriendCallbacks[iAr].EndInvoke(iAr);
                RemoveFriendCallbacks.Remove(iAr);
            }
        }

        public void UpdateProfile(string Motto, bool HomepageVisible, bool FriendRequestAllowed, bool ShowOnlineStatus, bool OfflineMessaging, bool FriendsCanFollow)
        {
            using (WebClientEx WC = new WebClientEx(Cookies))
            {
                NameValueCollection Arguments = new NameValueCollection(9);
                Arguments.Add("__app_key", CSRFToken);
                Arguments.Add("urlToken", URLToken);
                Arguments.Add("tab", "2");
                Arguments.Add("motto", Motto);
                Arguments.Add("visibility", HomepageVisible ? "EVERYONE" : "NOBODY");
                Arguments.Add("friendRequestsAllowed", FriendRequestAllowed.ToString().ToLower());
                Arguments.Add("showOnlineStatus", ShowOnlineStatus.ToString().ToLower());
                Arguments.Add("persistentMessagingAllowed", OfflineMessaging.ToString().ToLower());
                Arguments.Add("followFriendMode", Convert.ToByte(FriendsCanFollow).ToString());

                WC.Headers["X-App-Key"] = CSRFToken;
                WC.Headers["User-Agent"] = SKore.ChromeAgent;
                WC.UploadValues(Hotel.ToURL(true) + "/profile/profileupdate", Arguments);
            }
        }
        public IAsyncResult BeginUpdateProfile(string Motto, bool HomepageVisible, bool FriendRequestAllowed, bool ShowOnlineStatus, bool OfflineMessaging, bool FriendsCanFollow, AsyncCallback callback, object state)
        {
            IAsyncResult iAr = _UpdateProfile.BeginInvoke(Motto, HomepageVisible, FriendRequestAllowed, ShowOnlineStatus, OfflineMessaging, FriendsCanFollow, callback, state);
            UpdateProfileCallbacks.Add(iAr, _UpdateProfile);
            return iAr;
        }
        public void EndUpdateProfile(IAsyncResult iAr)
        {
            if (UpdateProfileCallbacks.ContainsKey(iAr))
            {
                UpdateProfileCallbacks[iAr].EndInvoke(iAr);
                UpdateProfileCallbacks.Remove(iAr);
            }
        }

        public void Flush()
        {
            _URLToken = string.Empty;
            _CSRFToken = string.Empty;
            _PlayerName = string.Empty;
            _Motto = string.Empty;
            _LastSignIn = string.Empty;
            _CreatedOn = string.Empty;
            _Gender = HGenders.Unknown;
            _Age = 0;
            _UserHash = string.Empty;
            ClientStarting = string.Empty;
            _GameData = HGameData.Empty;
            _FlashClientURL = string.Empty;
            _FlashClientRevision = string.Empty;
            _Port = 0;
            _Host = string.Empty;
            _AddFriend = null;
            _SSOTicket = string.Empty;
        }
        public void Refresh()
        {
            LoadResource(HPages.Me);
            LoadResource(HPages.Home);
            LoadResource(HPages.Client);
            LoadResource(HPages.Profile);
        }
        public string GenerateSSOTicket()
        {
            LoadResource(HPages.Client);
            return _SSOTicket;
        }
        public void DownloadClient(string Filename)
        {
            using (WebClientEx WC = new WebClientEx(Cookies))
            {
                WC.Headers["User-Agent"] = SKore.ChromeAgent;
                WC.DownloadFile(FlashClientURL, Filename);
            }
        }

        public void Connect()
        {
            ServerS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerS.BeginConnect(Addresses[0], Port, Connected, null);
        }
        public void Disconnect(bool Dispose = true)
        {
            lock (DisconnectLock)
            {
                if (ServerS == null) return;

                ServerS.Shutdown(SocketShutdown.Both);
                ServerS.Dispose();
                ServerS = null;

                if (Dispose)
                {
                    SKore.Unsubscribe(ref OnConnected);
                    SKore.Unsubscribe(ref DataToClient);
                    SKore.Unsubscribe(ref DataToServer);
                    SKore.Unsubscribe(ref OnDisconnected);
                }

                _ReceiveData = false;
                ServerB = ServerC = null;
                ToClientS = ToServerS = 0;

                if (OnDisconnected != null)
                    OnDisconnected(this, EventArgs.Empty);
            }
        }
        public int SendToServer(byte[] Data)
        {
            if (IsConnected)
            {
                if (DataToServer != null)
                {
                    try { DataToServer(this, new DataToEventArgs(Data, Protocol.HDestinations.Server, ++ToServerS)); }
                    catch { }
                }

                if (ClientEncrypt != null)
                    ClientEncrypt.Parse(Data);

                try
                {
                    ServerS.BeginSend(Data, 0, Data.Length, SocketFlags.None, FinishSending, null);
                }
                catch { Disconnect(); return 0; }

                return Data.Length;
            }
            else return 0;
        }
        public int SendToServer(ushort Header, params object[] Chunks)
        {
            return SendToServer(HMessage.Construct(Header, HDestinations.Server, HProtocols.Modern, Chunks));
        }
        int IHServer.SendToClient(byte[] Data)
        { return 0; }

        public void Dispose()
        {
            Flush();
            Disconnect(true);
        }
        #endregion

        #region Private Methods
        private string LoadResource(HPages Page)
        {
            using (WebClientEx WC = new WebClientEx(Cookies))
            {
                string Body = string.Empty;
                string URL = Page.Juice(Hotel) + (Page == HPages.Home ? PlayerName : string.Empty);
                WC.Headers["User-Agent"] = SKore.ChromeAgent;
                Body = WC.DownloadString(URL);
                HandleResource(Page, ref Body);
                return Body;
            }
        }
        private void HandleResource(HPages Page, ref string Body)
        {
            PlayerID = int.Parse(Body.GetChild("var habboId = ", ';'));
            _PlayerName = Body.GetChild("var habboName = \"", '\"');
            _Age = int.Parse(Body.GetChild("kvage=", ';'));
            _Gender = (HGenders)Char.ToUpper(Body.GetChild("kvgender=", ';')[0]);
            _CSRFToken = Body.GetChild("<meta name=\"csrf-token\" content=\"", '\"');

            switch (Page)
            {
                case HPages.Me:
                {
                    string[] InfoBoxes = Body.GetChilds("<div class=\"content\">", '<');
                    _Motto = InfoBoxes[6].Split('>')[1];
                    _LastSignIn = InfoBoxes[12].Split('>')[1];
                    break;
                }
                case HPages.Home:
                {
                    _CreatedOn = Body.GetChild("<div class=\"birthday date\">", '<');
                    _Motto = Body.GetChild("<div class=\"profile-motto\">", '<');
                    break;
                }
                case HPages.Profile:
                {
                    _URLToken = Body.GetChild("name=\"urlToken\" value=\"", '\"');
                    break;
                }
                case HPages.Client:
                {
                    _Host = Body.GetChild("\"connection.info.host\" : \"", '\"');
                    _Port = int.Parse(Body.GetChild("\"connection.info.port\" : \"", '\"').Split(',')[0]);
                    _Addresses = Dns.GetHostAddresses(_Host).Select(IP => IP.ToString()).ToArray();
                    _SSOTicket = Body.GetChild("\"sso.ticket\" : \"", '\"');

                    if (string.IsNullOrEmpty(ClientStarting)) ClientStarting = Body.GetChild("\"client.starting\" : \"", '\"');
                    else Body = Body.Replace(Body.GetChild("\"client.starting\" : \"", '\"'), ClientStarting);

                    _UserHash = Body.GetChild("\"user.hash\" : \"", '\"');
                    _GameData = HGameData.Parse(Body);

                    _FlashClientURL = "http://" + Body.GetChild("\"flash.client.url\" : \"", '\"').Substring(3) + "Habbo.swf";
                    _FlashClientRevision = _FlashClientURL.Split('/')[4];
                    break;
                }
            }
        }

        private void Connected(IAsyncResult iAr)
        {
            ServerS.EndConnect(iAr);
            ServerB = new byte[1024];

            _ReceiveData = true;
            ReadServerData();

            if (OnConnected != null)
                OnConnected(this, EventArgs.Empty);
        }
        private void ReadServerData()
        {
            if (IsConnected && ReceiveData)
                ServerS.BeginReceive(ServerB, 0, ServerB.Length, SocketFlags.None, DataFromServer, null);
        }
        private void DataFromServer(IAsyncResult iAr)
        {
            try
            {
                if (ServerS == null) return;

                int Length = ServerS.EndReceive(iAr);
                if (Length > 0)
                {
                    byte[] Data = ByteUtils.CopyBlock(ServerB, 0, Length);
                    try
                    {
                        byte[][] Chunks = new byte[][] { Data };

                        if (ServerDecrypt != null)
                            ServerDecrypt.Parse(Chunks[0]);

                        if (!ResponseEncrypted)
                            Chunks = ByteUtils.Split(ref ServerC, Data, HDestinations.Client, HProtocols.Modern);

                        if (ToClientS == 2)
                            ResponseEncrypted = (Chunks[0].Length - 4 != BigEndian.DecypherInt(Chunks[0]));

                        foreach (byte[] Chunk in Chunks)
                        {
                            ++ToClientS;
                            if (DataToClient != null)
                            {
                                try { DataToClient(this, new DataToEventArgs(Chunk, HDestinations.Client, ToClientS)); }
                                catch { }
                            }
                        }
                    }
                    catch { }
                    ReadServerData();
                }
                else Disconnect();
            }
            catch { Disconnect(); }
        }
        private void FinishSending(IAsyncResult iAr)
        {
            ServerS.EndSend(iAr);
        }
        #endregion

        #region Static Methods
        public static HSession[] Extract(string Path, char Delimiter = ':')
        {
            string[] Lines = File.ReadAllLines(Path);
            Lines = Lines.Where(S => !string.IsNullOrEmpty(S)).ToArray();
            HSession[] Sessions = new HSession[Lines.Length];

            for (int i = 0; i < Lines.Length; i++)
            {
                string[] Credentials = Lines[i].Split(Delimiter);
                Sessions[i] = new HSession(Credentials[0], Credentials[1], (HHotels)Enum.Parse(typeof(HHotels), Credentials[2].Replace('_', '.')));
            }
            return Sessions;
        }
        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion

        #region Instance Formatters
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", Email, Password, Hotel.ToDomain());
        }
        #endregion
    }
}