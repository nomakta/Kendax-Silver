using System;
using System.Net;

namespace Sulakore.Wrappers
{
    internal class WebClientEx : WebClient
    {
        private CookieContainer Cookies;

        public WebClientEx(CookieContainer Cookies)
            : base()
        {
            this.Cookies = Cookies;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest Request = base.GetWebRequest(address) as HttpWebRequest;
            if (Request == null) return base.GetWebRequest(address);
            Request.CookieContainer = Cookies;
            return Request;
        }
    }
}