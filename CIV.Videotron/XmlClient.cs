using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.IO;
using System.Threading;
using CIV.Common;
using System.Xml;
using System.Xml.Serialization;
using Videotron.Api.Xml;
using Videotron.Wired;
using Videotron.Exceptions;

namespace Videotron
{
    public delegate void ReadDailyWiredUsage(string username, WiredDailyUsage usage, ref bool stop);
    public delegate void XmlClientError(Exception exception);
    public delegate void DownloadDataEndEventHandler(WiredAccount wiredAccount);

    public class XmlClient
    {
        public ReadDailyWiredUsage OnReadDailyWiredUsage;
        public XmlClientError OnError;
        public event EventHandler OnDownloadData;
        public event DownloadDataEndEventHandler OnDownloadDataEnd;
        public event EventHandler OnDownloadHistory;
        public event EventHandler OnDownloadHistoryEnd;

        private const string _apiVersion = "1.0";
        private const string _apiUrl = "https://www.videotron.com/api/{0}/internet/usage/wired/{1}.xml?period={2}&lang={3}&caller=civ.codexmundus.com";

        private SupportedLanguages _language;
        public string Token { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }

        public Exception Error
        {
            private set;
            get;
        }

        public XmlClient(SupportedLanguages language,
                         string token,
                         string username)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            _language = language;
            Token = token;
            Username = username;
        }

        public XmlClient(SupportedLanguages language,
                         string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            _language = language;
            Token = token;
        }

        protected void DoReadDailyWiredUsage(string username, WiredDailyUsage usage, ref bool stop)
        {
            if (OnReadDailyWiredUsage != null)
                OnReadDailyWiredUsage(username, usage, ref stop);
        }

        protected void DoError(Exception exception)
        {
            Error = exception;

            if (OnError != null)
                OnError(exception);
        }

        protected void DoDownloadtHistory()
        {
            if (OnDownloadHistory != null)
                OnDownloadHistory(this, null);
        }

        protected void DoDownloadtHistoryEnd()
        {
            if (OnDownloadHistoryEnd != null)
                OnDownloadHistoryEnd(this, null);
        }

        protected void DoDownloadData()
        {
            if (OnDownloadData != null)
                OnDownloadData(this, null);
        }

        protected void DoDownloadDataEnd(WiredAccount wiredAccount)
        {
            if (OnDownloadDataEnd != null)
                OnDownloadDataEnd(wiredAccount);
        }

        private string ApiUrl(int period)
        {
            return String.Format(_apiUrl,
                                 _apiVersion,
                                 Token,
                                 period,
                                 _language == SupportedLanguages.French ? "fr" : "en");
        }

        private HttpWebRequest CreateRequest(string url)
        {
            HttpWebRequest newRequest = (HttpWebRequest)WebRequest.Create(url);

            newRequest.Timeout = 60000;
            newRequest.Host = "www.videotron.com";
            //newRequest.Host = "codexmundus.net";
            newRequest.ProtocolVersion = HttpVersion.Version10;
            newRequest.UserAgent = "CIV";
            newRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            newRequest.Proxy = HttpWebRequest.DefaultWebProxy;
            newRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            newRequest.KeepAlive = true;
            newRequest.AllowAutoRedirect = true;
            newRequest.Method = "GET";

            return newRequest;
        }

        private bool GetResponse(HttpWebRequest request, out string source)
        {
            StreamReader stream;
            int retries = 5;

            TryAgain:

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    stream = new StreamReader(response.GetResponseStream(), UTF8Encoding.UTF8);
                    source = stream.ReadToEnd();

                    return true;
                }
            }
            catch (WebException web)
            {
                // C'est possible que ça corrige certaine erreur de connexion
                if ((web.Status == WebExceptionStatus.ConnectFailure) || (web.Status == WebExceptionStatus.NameResolutionFailure))
                {
                    if (0 < --retries)
                    {
                        Thread.Sleep(1000);
                        goto TryAgain;
                    }
                }
                throw;
            }
            catch (Exception dwl)
            {
                DownloadException downloadException = new DownloadException(null, dwl);
                downloadException.OriginException = dwl;
                DoError(downloadException);
                source = string.Empty;
                return false;
            }
        }

        private bool Parse(string source, bool updateClient, out WiredAccount wiredAccount)
        {

            WiredInternetUsage wiredInternetUsage;

            try
            {
                // Convertir la source en objet
                using (XmlReader r = XmlReader.Create(new StringReader(source)))
                {
                    XmlSerializer s = new XmlSerializer(typeof(WiredInternetUsage));
                    wiredInternetUsage = (WiredInternetUsage)s.Deserialize(r);
                }
            }
            catch (Exception seria)
            {
                wiredAccount = null;
                ParseException parseException = new ParseException(null, seria);
                parseException.OriginException = seria;
                DoError(parseException);
                return false;
            }

            Message error = wiredInternetUsage.Messages.Message.FirstOrDefault(x => x.Severity == "error");
            if (error != null)
            {
                throw new ApiException(error.Text);
            }

            // Transformation des données
            wiredAccount = WiredAccountFactory.CreateWiredAccount(wiredInternetUsage, Username);
            return true;
        }

        private void ReadDailyWiredUsage(List<WiredDailyUsage> dailyUsage, ref bool stop)
        {
            for (int i = dailyUsage.Count - 1; i >= 0; i--)
            {
                DoReadDailyWiredUsage(Username, dailyUsage[i], ref stop);
                if (stop)
                    break;
            }
        }

        public List<string> GetUsernameByToken(out string message)
        {
            message = string.Empty;

            List<string> result = new List<string>();

            string sourcePage;
            HttpWebRequest request = CreateRequest(ApiUrl(0));

            if (GetResponse(request, out sourcePage))
            {

                WiredInternetUsage wiredInternetUsage;

                // Convertir la source en objet
                using (XmlReader r = XmlReader.Create(new StringReader(sourcePage)))
                {
                    XmlSerializer s = new XmlSerializer(typeof(WiredInternetUsage));
                    wiredInternetUsage = (WiredInternetUsage)s.Deserialize(r);
                }

                foreach (WiredInternetAccountUsage item in wiredInternetUsage.InternetAccounts.WiredInternetAccountUsage)
                    result.Add(item.InternetAccountNo);

                Message error = wiredInternetUsage.Messages.Message.FirstOrDefault(x => x.Severity == "error");
                if (error != null)
                    message = error.Text;
            }
            return result;
        }

        public void Execute()
        {
            Error = null;
            Success = false;

            try
            {
                bool stop = false;
                string sourcePage;
                HttpWebRequest request;
                WiredAccount wiredAccount;

                DoDownloadData();
                request = CreateRequest(ApiUrl(0));

                if (GetResponse(request, out sourcePage))
                {
                    // Crée les objets en fonction du document xml
                    if (Parse(sourcePage, true, out wiredAccount))
                    {
                        DoDownloadDataEnd(wiredAccount);

                        // Historique de la consommation de la période courante
                        ReadDailyWiredUsage(wiredAccount.DailyUsage, ref stop);

                        DoDownloadtHistoryEnd();

                        // S'il faut aller chercher l'historique des périodes précédentes
                        if (!stop)
                        {
                            for (int i = -1; i >= -3; i--)
                            {
                                DoDownloadtHistory();
                                request = CreateRequest(ApiUrl(i));
                                if (GetResponse(request, out sourcePage))
                                {
                                    if (Parse(sourcePage, true, out wiredAccount))
                                    {
                                        ReadDailyWiredUsage(wiredAccount.DailyUsage, ref stop);

                                        if (stop)
                                            break;
                                    }
                                }
                                else
                                    break;
                            }
                        }
                        Success = true;
                    }
                }
            }
            catch (Exception exec)
            {
                DoError(exec);
            }
        }
    }
}
