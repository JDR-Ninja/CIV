using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.IO;
using System.Text.RegularExpressions;
using CIV.Common;
using System.Threading;

namespace Videotron
{
    public delegate void ReadDailyUsage(string username, VideotronDailyUsage usage, ref bool continu);
    public delegate void BrowserError(Exception exception);
    public delegate void DownloadHistoryEventHandler(DateTime start, DateTime end);

    public class Browser
    {
        public ReadDailyUsage OnReadDailyUsage;
        public BrowserError OnError;
        public event EventHandler OnLogin;
        public event EventHandler OnDownloadData;
        public event EventHandler OnDownloadDataEnd;
        public event DownloadHistoryEventHandler OnDownloadHistory;

        private bool _success;

        public bool Success
        {
            get { return _success; }
            private set { _success = value; }
        }

        private VideotronAccount _client;
        private CookieCollection _cookies;
        private string _userAgent;

        public string ErrorCode;

        private Exception _error;

        public Exception Error
        {
            get { return _error; }
        }

        protected void DoLogin()
        {
            if (OnLogin != null)
                OnLogin(this, null);
        }

        protected void DoDownloadtHistory(DateTime start, DateTime end)
        {
            if (OnDownloadHistory != null)
                OnDownloadHistory(start, end);
        }

        protected void DoDownloadData()
        {
            if (OnDownloadData != null)
                OnDownloadData(this, null);
        }

        protected void DoDownloadDataEnd()
        {
            if (OnDownloadDataEnd != null)
                OnDownloadDataEnd(this, null);
        }

        protected void DoReadDailyUsage(string username, VideotronDailyUsage usage, ref bool continu)
        {
            if (OnReadDailyUsage != null)
                OnReadDailyUsage(username, usage, ref continu);
        }

        protected void DoError(Exception exception)
        {
            _error = exception;

            if (OnError != null)
                OnError(exception);
        }

        public Browser()
        {
            Success = true;
        }

        /// <summary>
        /// Met à jours les données d'un client avec celles disponible sur internet
        /// </summary>
        /// <param name="client"></param>
        public bool Execute(VideotronAccount client)
        {
            Success = false;

            _error = null;

            _client = client;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

            try
            {
                GetInternetUsage();
                Success = true;
            }
            catch (Exception requestError)
            {
                DoError(requestError);
                _error = requestError;
            }

            return Success;
        }

        public void GetInternetUsage()
        {
            List<Period> period;
            bool updateHistory;
            string sourcePage;
            HttpWebRequest request;

            DoLogin();

            request = CreateRequest(String.Format("https://www.videotron.com/client/secur/ConsommationInternet.do?standardFlow=true&vl={0}", _client.Username), String.Empty, String.Empty);
            
            GetResponse(request, out sourcePage);


            //File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("hh.mm.ss") + ".html"), sourcePage);

            //string username = UsagePageDecoder.GetUsername(sourcePage);

            // Pas de nom d'utilisateur
            /*if (username == null)
                // changer ça en WebResponseException
                //throw new AuthentificationFailedException();
                throw new VideotronException() { Status = VideotronExceptionStatus.AuthentificationFailed };

            // Nom d'utilisateur différent (croisement de donnée, compte multiple)
            else if (_client.Username != username)
                GetInternetUsage();
            else
            {*/
                DoDownloadData();

                request = CreateRequest("https://www.videotron.com/client/secur/CIUserSecurise.do",
                                        String.Format("https://www.videotron.com/client/secur/ConsommationInternet.do?standardFlow=true&vl={0}", _client.Username),
                                        String.Format("showDetails=true&vl={0}&dataVolumeUnitCode=KB&billingPeriodString=0&direction=total", _client.Username));
                                        //String.Format("standardFlow=true&tabId=tabId_0&cable.showDetails=true&vl={0}&cable.conversionRatio=1&lang=fr&cable.billingPeriodString=0&cable.usage=total", _client.Username));

                GetResponse(request, out sourcePage);

                /*username = UsagePageDecoder.GetUsername(sourcePage);

                // Pas de nom d'utilisateur
                if (username == null)
                    throw new VideotronException() { Status = VideotronExceptionStatus.AuthentificationFailed };
                // Nom d'utilisateur différent (croisement de donnée, compte multiple)
                else if (_client.Username != username)
                    GetInternetUsage();
                else
                {*/
                    //File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("hh.mm.ss") + ".html"), sourcePage);
                    Analyze(sourcePage, true, out period, out updateHistory);

                    DoDownloadDataEnd();

                    if (updateHistory)
                    {
                        List<Period> histperiod;
                        for (int i = 1; i < period.Count; i++)
                        {
                            // Ne pas faire la période courante

                            DoDownloadtHistory(period[i].Start, period[i].End);

                            request = CreateRequest("https://www.videotron.com/client/secur/CIUserSecurise.do",
                                                    "https://www.videotron.com/client/secur/CIUserSecurise.do",
                            //                        String.Format("standardFlow=true&tabId=tabId_0&cable.showDetails=true&vl={0}&cable.conversionRatio=1&lang=fr&cable.billingPeriodString={1}&cable.usage=total", _client.Username, i));
                                                    String.Format("showDetails=true&vl={0}&dataVolumeUnitCode=KB&billingPeriodString={1}&direction=total&displaySubmit=Submit", _client.Username, i));

                            // Il faut caller une seconde fois pour avoir les données en ko
                            //request = CreateRequest("https://www.videotron.com/client/secur/CIUserSecurise.do",
                            //                        "https://www.videotron.com/client/secur/CIUserSecurise.do",
                            //                        String.Format("standardFlow=true&tabId=tabId_0&cable.showDetails=true&vl={0}&cable.conversionRatio=1&lang=fr&cable.billingPeriodString={1}&cable.usage=total", _client.Username, i));

                            // Boucler au cas où il y aurait un croisement de donnée (compte multiple)
                            /*do
                            {
                                GetResponse(request, out sourcePage);
                            }
                            while (!UsagePageDecoder.ValidUsername(sourcePage, _client.Username));*/

                            GetResponse(request, out sourcePage);
                            Analyze(sourcePage, false, out histperiod, out updateHistory);

                            if (!updateHistory)
                                break;
                        }
                    }
                    
                //}
            //}
        }

        private bool Analyze(string source, bool updateClient, out List<Period> period, out bool updateHistory)
        {
            //File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMddHHmmssf")) + ".html", source);
            updateHistory = true;

            UsagePageDecoder page = new UsagePageDecoder();

            if (page.Extract(source))
            {
                if (updateClient)
                {
                    _client.PeriodStart = page.PeriodStart;
                    _client.PeriodEnd = page.PeriodEnd.AddDays(1).AddMinutes(-1);
                    _client.Download = page.Download;
                    _client.Upload = page.Upload;
                    //_client.CurrentCombined = page.TotalCombined;
                }

                period = page.BillPeriods;

                // Enregistrement de la consommation
                for (int i = page.DailyUsages.Count - 1; i >= 0; i--)
                {
                    // Ne pas enregistrer la date courante, car elle n'est pas encore finale
                    if (page.DailyUsages[i].Day.Date.CompareTo(DateTime.Today.Date) != 0)
                    {
                        DoReadDailyUsage(_client.Username, page.DailyUsages[i], ref updateHistory);
                        if (!updateHistory)
                            break;
                    }
                }

                return true;
            }
            else
            {
                period = new List<Period>();
                updateHistory = false;
                if (page.decodeException != null)
                    DoError(page.decodeException);
                return false;
            }
        }

        private bool GetResponse(HttpWebRequest request, out string source)
        {
            StreamReader stream;
            StringBuilder result = new StringBuilder();

            int retries = 5;

            TryAgain:

            try
            {
                //throw new WebException("pouet", WebExceptionStatus.ConnectFailure);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.Cookies.Count > 0)
                        _cookies = response.Cookies;

                    if (String.IsNullOrEmpty(response.ContentEncoding))
                        stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252));
                    else
                    {
                        try
                        {
                            stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(response.ContentEncoding));
                        }
                        catch
                        {
                            stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252));
                        }
                    }

                    result.Append(stream.ReadToEnd());
                }
            }
            catch(WebException ex)
            {
                // C'est possible que ça corrige certaine erreur de connexion
                if ((ex.Status == WebExceptionStatus.ConnectFailure) || (ex.Status == WebExceptionStatus.NameResolutionFailure))
                {
                    if (0 < --retries)
                    {
                        DoError(ex);
                        Thread.Sleep(2000);
                        goto TryAgain;
                    }
                }
                throw ex;
            }


            if (result.Length == 0)
                throw new VideotronException() { Status = VideotronExceptionStatus.ResponseEmpty};

            //File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMddHHmmssf")) + ".html", result.ToString());

            source = result.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
            return true;
        }

        private string CreateUserAgent()
        {
            if (String.IsNullOrEmpty(_userAgent))
                _userAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; fr; rv:1.9.2.8) Gecko/20100722 Firefox/3.6.8";

            return _userAgent;
        }

        private HttpWebRequest CreateRequest(string url, string referer, string postData)
        {
            HttpWebRequest newRequest = (HttpWebRequest)WebRequest.Create(url);
            newRequest.Timeout = 60000;
            newRequest.Host = "www.videotron.com";
            newRequest.Referer = referer;
            newRequest.ProtocolVersion = HttpVersion.Version10;
            newRequest.UserAgent = CreateUserAgent();
            newRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            newRequest.Proxy = HttpWebRequest.DefaultWebProxy;
            newRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            newRequest.KeepAlive = true;
            newRequest.AllowAutoRedirect = true;

            newRequest.CookieContainer = new CookieContainer();

            if (_cookies != null && _cookies.Count > 0)
                newRequest.CookieContainer.Add(_cookies);

            if (!String.IsNullOrEmpty(postData))
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(postData);
                newRequest.ContentType = "application/x-www-form-urlencoded";
                newRequest.Method = "POST";
                newRequest.ContentLength = data.Length;

                Stream newStream = newRequest.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();
            }

            return newRequest;
        }
    }
}
