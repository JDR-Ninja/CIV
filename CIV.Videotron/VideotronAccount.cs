using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CIV.Common;
using System.Timers;

namespace Videotron
{

    public class VideotronAccount : INotifyPropertyChanged, IEditableObject
    {
        struct AccountData
        {
            internal double Upload;
            internal double Download;
            internal double TotalCombined;
        }

        private System.Timers.Timer _timer;

        private AccountData backupData;

        public bool NewDataAvailable
        {
            get
            {
                return backupData.Upload != Upload || backupData.Download != Download || backupData.TotalCombined != Combined;
            }
        }

        /// <summary>
        /// Identification
        /// </summary>
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; Notify("Name"); Notify("DisplayName"); }
        }

        /// <summary>
        /// Le nom du compte. Pour usage interne seulement
        /// </summary>
        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {

                if (Name == Username && !String.IsNullOrEmpty(Username))
                    Name = value;

                _username = value != null ? value.ToUpper() : String.Empty;

                Notify("Username");
                Notify("DisplayName");
            }
        }

        // Service internet
        private string _userInternetAccess;

        public string UserInternetAccess
        {
            get { return _userInternetAccess; }
            set { _userInternetAccess = value; Notify("UserInternetAccess"); }
        }

        private DateTime _lastUpdate;

        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public TimeSpan DayRemaining
        {
            get
            {
                if (PeriodEnd > DateTime.Now)
                    return PeriodEnd.Subtract(DateTime.Now);
                else
                    return new TimeSpan();
            }
        }

        public TimeSpan DayElapsed
        {
            get
            {
                if (PeriodStart < DateTime.Now)
                    return DateTime.Now.Subtract(PeriodStart);
                else
                    return new TimeSpan();
            }
        }

        public double CurrentDayUpload { get; set; }

        public double CurrentDayDownload { get; set; }

        public double CurrentDayCombined
        {
            get
            {
                return CurrentDayDownload + CurrentDayUpload;
            }
        }

        // Consommation
        public double MaxUpload { set; get; }
        public double Upload { set; get; }
        public double UploadPercent
        {
            get
            {
                double result = Upload / CombinedMaximum;

                if (!Double.IsNaN(result) && !Double.IsInfinity(result))
                    return result;
                else
                    return 0;
            }
        }

        public double MaxDownload { set; get; }
        public double Download { set; get; }
        public double DownloadPercent
        {
            get
            {
                double result = Download / CombinedMaximum;

                if (!Double.IsNaN(result) && !Double.IsInfinity(result))
                    return result;
                else
                    return 0;
            }
        }

        public double Combined
        {
            get
            {
                return Download + Upload;
            }
        }

        public double CombinedPercent
        {
            get
            {
                double result = Combined / CombinedMaximum;

                if (!Double.IsNaN(result) && !Double.IsInfinity(result))
                    return result;
                else
                    return 0;
            }
        }

        public double CombinedRemaining
        {
            get
            {
                double result = CombinedMaximum - Combined;
                if (result < 0)
                    return 0;
                else
                    return result;
            }
        }

        /// <summary> 
        /// Le montant de la surcharge
        /// </summary>
        public double Overcharge
        {
            get
            {
                if (InternetAccesList.Instance[UserInternetAccess] != null)
                {
                    if (Combined > CombinedMaximum)
                    {
                        double result = System.Math.Round(((Combined - CombinedMaximum) / 1048576) * InternetAccesList.Instance[UserInternetAccess].OverCharge, 2);
                        if (InternetAccesList.Instance[UserInternetAccess].MaxCost != 0 && result > InternetAccesList.Instance[UserInternetAccess].MaxCost)
                            return InternetAccesList.Instance[UserInternetAccess].MaxCost;
                        else
                            return result;
                    }
                }
                return 0;
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Notify("DayRemaining");
        }

        public VideotronAccount()
        {
            Username = "VL";

            CurrentDayUpload = 0;
            CurrentDayDownload = 0;

            _timer = new System.Timers.Timer(60000);
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Le nom à afficher
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (!String.IsNullOrEmpty(Name))
                    return Name;
                else
                    return Username;
            }
        }

        private double _combinedMaximum;
        public double CombinedMaximum
        {
            get {return _combinedMaximum;}
            set
            {
                _combinedMaximum = value; Notify("CombinedMaximum");
            }
        }

        /// <summary>
        /// Consommation combinée quotidienne suggérée
        /// </summary>
        public double SuggestCombined
        {
            get
            {
                double result = CombinedRemaining + CurrentDayCombined;

                // Si c'est dans la dernière journée
                if (DayRemaining.Days == 0 && DayRemaining.TotalSeconds > 0)
                    return CombinedRemaining + CurrentDayCombined;

                // S'il reste encore des jours
                else if (DayRemaining.Days > 0)
                {
                    int days = DayRemaining.Days;

                    if (DayRemaining.Hours > 0)
                        days++;

                    return result > 0 ? result / days : 0;
                }

                // La période est passé, calcul impossible, il faut rafraichir les données
                else
                    return 0;
            }
        }

        /// <summary>
        /// Consommation combinée quotidienne suggérée en pourcentage
        /// </summary>
        public double SuggestCombinedPercent
        {
            get
            {
                double result = SuggestCombined / CombinedMaximum;

                if (result < 0)
                    return 0.0;
                else
                    return result;
            }
        }

        /// <summary>
        /// Estimation de la consommation combinée total mensuel
        /// </summary>
        public double EstimateTotalCombined
        {
            get
            {
                return CombinedMaximum - EstimateCombined;
            }
        }

        /// <summary>
        /// Estimation de ce que sera votre Consommation Combinée à la fin de votre période. 
        /// </summary>
        public double EstimateCombined
        {
            get
            {
                return Combined + (AverageCombined * DayRemaining.Days);
            }
        }

        /// <summary>
        /// Consommation combinée moyenne
        /// </summary>
        public double AverageCombined
        {
            get
            {
                if (DayElapsed.Days > 0)
                    return Combined / DayElapsed.Days;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Consommation quotidienne combinée théorique
        /// </summary>
        public double TheoryDailyCombined
        {
            get
            {
                if (PeriodEnd.Subtract(PeriodStart).Days > 0)
                    return CombinedMaximum / PeriodEnd.Subtract(PeriodStart).Days;
                else
                    return 0;
            }
        }

        public double TheoryDailyCombinedPercent
        {
            get
            {
                double result = TheoryDailyCombined / CombinedMaximum;

                if (result < 0)
                    return 0.0;
                else
                    return result;
            }
        }

        /// <summary>
        /// Consommation combinée théorique
        /// </summary>
        public double TheoryCombined
        {
            get
            {
                return TheoryDailyCombined * DayElapsed.Days;
            }
        }

        /// <summary>
        /// Différence entre la consommation combinée théorique et réelle
        /// </summary>
        public double TheoryCombinedVersusCombined
        {
            get
            {
                return TheoryCombined - Combined;
            }
        }

        public int CurrentUsageDayAdvance
        {
            get
            {
                if (TheoryDailyCombined > 0)
                    return (int)System.Math.Round(TheoryCombinedVersusCombined / TheoryDailyCombined, 0);
                else
                    return 0;
            }
        }

        private string _token;

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }


        public void Reset()
        {
            Upload = 0;
            Download = 0;
            //CurrentCombined = 0;
        }

        /// <summary>
        /// Déclenche les notifications de toutes les propriétés
        /// </summary>
        public void Refresh()
        {
            Notify("PeriodStart");
            Notify("PeriodEnd");
            Notify("DayRemaining");
            Notify("DayElapsed");
            Notify("MaxUpload");
            Notify("Upload");
            Notify("UploadPercent");
            Notify("MaxDownload");
            Notify("Download");
            Notify("DownloadPercent");
            Notify("Combined");
            Notify("CombinedPercent");
            Notify("CombinedRemaining");
            Notify("Overcharge");
            Notify("CombinedMaximum");
            Notify("SuggestCombined");
            Notify("SuggestCombinedPercent");
            Notify("EstimateTotalCombined");
            Notify("EstimateCombined");
            Notify("AverageCombined");
            Notify("TheoryDailyCombined");
            Notify("TheoryDailyCombinedPercent");
            Notify("TheoryCombined");
            Notify("TheoryCombinedVersusCombined");
            Notify("CurrentUsageDayAdvance");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #region IEditableObject Members

        public void BeginEdit()
        {
            backupData.Upload = Upload;
            backupData.Download = Download;
            backupData.TotalCombined = Combined;
        }

        public void CancelEdit()
        {
            Upload = backupData.Upload;
            Download = backupData.Download;
            //CurrentCombined = backupData.TotalCombined;
        }

        public void EndEdit()
        {
            backupData.Upload = Upload;
            backupData.Download = Download;
            backupData.TotalCombined = Combined;
        }

        #endregion
    }
}
