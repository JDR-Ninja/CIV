using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Windows;
using CIV.Common;

namespace CIV
{
    [XmlRootAttribute(ElementName = "ProgramSettings", IsNullable = false)]
    public class ProgramSettings : INotifyPropertyChanged
    {
        public CIVVersion Version { get; set; }

        [XmlIgnore]
#if !DEBUG
        private static string _key = "eG8s0_Gu94HmoonbEgJl";
#endif
        private static ProgramSettings _instance;
        private static object _instanceLocker = new object();

        // Singleton
        public static ProgramSettings Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                        _instance = ProgramSettings.Load();
                    return _instance;
                }
            }
        }

        public static void Reload()
        {
            _instance = Load();
        }

        [XmlIgnore]
        private const string _filename = "Settings.dat";

        /// <summary>
        /// Identifiant
        /// </summary>
        private Guid _id;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private bool _isPortable;

        [XmlIgnore]
        public bool IsPortable
        {
            get { return _isPortable; }
            set { _isPortable = value; }
        }


        /// <summary>
        /// Date de création des options
        /// </summary>
        private DateTime _created;

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        private SupportedLanguages _userLanguage;

        public SupportedLanguages UserLanguage
        {
            get { return _userLanguage; }
            set { _userLanguage = value; }
        }

        /// <summary>
        /// Liste des comptes vidéotron
        /// </summary>
        private List<CIVAccount> _accounts;
        
        public List<CIVAccount> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; }
        }

        /// <summary>
        /// Lancer l'application au démarrage du système
        /// </summary>
        private bool _launchAtStartup;

        public bool LaunchAtStartup
        {
            get { return _launchAtStartup; }
            set { _launchAtStartup = value; Notify("LaunchAtStartup"); }
        }

        /// <summary>
        /// Lancer l'application en mode minimisé
        /// </summary>
        private bool _launchMinimized;

        public bool LaunchMinimized
        {
            get { return _launchMinimized; }
            set { _launchMinimized = value; Notify("LaunchMinimized"); }
        }

        /// <summary>
        /// Rafraichir la consommation automatiquement
        /// </summary>
        private bool _automaticClientUpdate;

        public bool AutomaticClientUpdate
        {
            get { return _automaticClientUpdate; }
            set { _automaticClientUpdate = value; Notify("AutomaticClientUpdate"); }
        }

        /// <summary>
        /// Dernière fois de la vérification de mise à jours
        /// </summary>
        private DateTime _nextAppAutoUpdate;

        public DateTime NextAppAutoUpdate
        {
            get { return _nextAppAutoUpdate; }
            set { _nextAppAutoUpdate = value; }
        }

        private DateTime _nextInternetAccesUpdate;

        public DateTime NextInternetAccesUpdate
        {
            get { return _nextInternetAccesUpdate; }
            set { _nextInternetAccesUpdate = value; Notify("NextInternetAccesUpdate"); }
        }

        /// <summary>
        /// Vérifier automatiquement la présence de mises à jours
        /// </summary>
        private bool _appAutoUpdate;

        public bool AppAutoUpdate
        {
            get { return _appAutoUpdate; }
            set { _appAutoUpdate = value; Notify("AppAutoUpdate"); }
        }

        private bool _isProtected;

        public bool IsProtected
        {
            get { return _isProtected; }
            set { _isProtected = value; }
        }

        private SIUnitTypes _showUnitType;

        public SIUnitTypes ShowUnitType
        {
            get { return _showUnitType; }
            set { _showUnitType = value; Notify("ShowUnitType"); }
        }

        private bool _automaticSendReport;

        public bool AutomaticSendReport
        {
            get { return _automaticSendReport; }
            set { _automaticSendReport = value; Notify("AutomaticSendReport"); }
        }

        private SmtpSettings _emailSMTP;

        public SmtpSettings EmailSMTP
        {
            get { return _emailSMTP; }
            set { _emailSMTP = value; Notify("EmailSMTP"); }
        }

        private ColorRGB _uploadColor;

        public ColorRGB UploadColor
        {
            get { return _uploadColor; }
            set { _uploadColor = value; Notify("UploadColor"); }
        }

        private ColorRGB _downloadColor;

        public ColorRGB DownloadColor
        {
            get { return _downloadColor; }
            set { _downloadColor = value; Notify("DownloadColor"); }
        }

        private ColorRGB _combinedColor;

        public ColorRGB CombinedColor
        {
            get { return _combinedColor; }
            set { _combinedColor = value; Notify("CombinedColor"); }
        }

        private bool _appUpdateAvailable;

        public bool AppUpdateAvailable
        {
            get { return _appUpdateAvailable; }
            set { _appUpdateAvailable = value; Notify("AppUpdateAvailable"); }
        }

        private CIVVersion _appUpdateVersion;

        public CIVVersion AppUpdateVersion
        {
            get { return _appUpdateVersion; }
            set { _appUpdateVersion = value; Notify("AppUpdateVersion"); }
        }

        private bool _displayHistoryGraph;

        public bool DisplayHistoryGraph
        {
            get { return _displayHistoryGraph; }
            set { _displayHistoryGraph = value; Notify("DisplayHistoryGraph"); }
        }

        private bool _displayUsagePeriod;

        public bool DisplayUsagePeriod
        {
            get { return _displayUsagePeriod; }
            set { _displayUsagePeriod = value; Notify("DisplayUsagePeriod"); }
        }

        private bool _displayOvercharge;

        public bool DisplayOvercharge
        {
            get { return _displayOvercharge; }
            set { _displayOvercharge = value; Notify("DisplayOvercharge"); }
        }

        private bool _displayDayRemaining;

        public bool DisplayDayRemaining
        {
            get { return _displayDayRemaining; }
            set { _displayDayRemaining = value; Notify("DisplayDayRemaining"); }
        }

        private bool _displayAverageCombined;

        public bool DisplayAverageCombined
        {
            get { return _displayAverageCombined; }
            set { _displayAverageCombined = value; Notify("DisplayAverageCombined"); }
        }

        private bool _displaySuggestCombined;

        public bool DisplaySuggestCombined
        {
            get { return _displaySuggestCombined; }
            set { _displaySuggestCombined = value; Notify("DisplaySuggestCombined"); }
        }

        private bool _displayEstimateCombined;

        public bool DisplayEstimateCombined
        {
            get { return _displayEstimateCombined; }
            set { _displayEstimateCombined = value; Notify("DisplayEstimateCombined"); }
        }

        private bool _displayEstimateTotalCombined;

        public bool DisplayEstimateTotalCombined
        {
            get { return _displayEstimateTotalCombined; }
            set { _displayEstimateTotalCombined = value; Notify("DisplayEstimateTotalCombined"); }
        }

        private bool _displaySuggestCombinedPercent;

        public bool DisplaySuggestCombinedPercent
        {
            get { return _displaySuggestCombinedPercent; }
            set { _displaySuggestCombinedPercent = value; Notify("DisplaySuggestCombinedPercent"); }
        }

        private bool _displayTheoryDailyCombined;

        public bool DisplayTheoryDailyCombined
        {
            get { return _displayTheoryDailyCombined; }
            set { _displayTheoryDailyCombined = value; Notify("DisplayTheoryDailyCombined"); }
        }

        private bool _displayTheoryCombined;

        public bool DisplayTheoryCombined
        {
            get { return _displayTheoryCombined; }
            set { _displayTheoryCombined = value; Notify("DisplayTheoryCombined"); }
        }

        private bool _displayTheoryCombinedDifference;

        public bool DisplayTheoryCombinedDifference
        {
            get { return _displayTheoryCombinedDifference; }
            set { _displayTheoryCombinedDifference = value; Notify("DisplayTheoryCombinedDifference"); }
        }

        private bool _displayUploadAndDownload;

        public bool DisplayUploadAndDownload
        {
            get { return _displayUploadAndDownload; }
            set { _displayUploadAndDownload = value; Notify("DisplayUploadAndDownload"); }
        }

        private bool _displayCombined;

        public bool DisplayCombined
        {
            get { return _displayCombined; }
            set { _displayCombined = value; Notify("DisplayCombined"); }
        }

        private bool _displayCombinedGraph;

        public bool DisplayCombinedGraph
        {
            get { return _displayCombinedGraph; }
            set { _displayCombinedGraph = value; Notify("DisplayCombinedGraph"); }
        }
        
        private Point _dashbordPosition;

        public Point DashboardPosition
        {
            get { return _dashbordPosition; }
            set { _dashbordPosition = value; }
        }

        private bool _minimizeToTrayOnClose;

        public bool MinimizeToTrayOnClose
        {
            get { return _minimizeToTrayOnClose; }
            set { _minimizeToTrayOnClose = value; Notify("MinimizeToTrayOnClose"); }
        }

        public List<DisplayInfoTypes> Display { get; set; }

        public List<DisplayInfoTypes> DisplaySystray { get; set; }

        private bool _saveDisplayPosition;

        public bool SaveDisplayPosition
        {
            get { return _saveDisplayPosition; }
            set { _saveDisplayPosition = value; Notify("SaveDisplayPosition"); }
        }

        private bool _systrayDisplay;

        public bool SystrayDisplay
        {
            get { return _systrayDisplay; }
            set { _systrayDisplay = value; Notify("SystrayDisplay"); }
        }

        public ProgramSettings()
        {
            Id = Guid.NewGuid();
            Accounts = new List<CIVAccount>();
            IsProtected = false;
            ShowUnitType = SIUnitTypes.Mo;
            Created = DateTime.Now;
            EmailSMTP = new SmtpSettings();
            UserLanguage = SupportedLanguages.French;
            AutomaticSendReport = true;
            UploadColor = new ColorRGB(8, 141, 182);
            DownloadColor = new ColorRGB(221, 167, 15);
            CombinedColor = new ColorRGB(0, 100, 50);
            NextInternetAccesUpdate = DateTime.Now.AddDays(14);
            DisplayHistoryGraph = true;
            DashboardPosition = new Point();
            MinimizeToTrayOnClose = false;
            SaveDisplayPosition = false;
            SystrayDisplay = true;

            // Affichage par défaut
            Display = new List<DisplayInfoTypes>();
            DisplaySystray = new List<DisplayInfoTypes>();

            Version = new CIVVersion("3.5.0.0");

            IsPortable = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portable"));
        }

        public bool Save()
        {
            try
            {
                #if DEBUG
                XmlFactory.SaveToFile(this, typeof(ProgramSettings), Filename, new XmlFactorySettings());
                #else
                    XmlFactory.SaveToFile(this, typeof(ProgramSettings), Filename, new XmlFactorySettings() { Password = _key });
#endif

                return true;
            }
            catch (Exception saveException)
            {
                LogFactory.LogEngine.Instance.Add(saveException, false);
                MessageBox.Show(saveException.Message, "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static ProgramSettings Load()
        {
            ProgramSettings result;

            if (File.Exists(Filename))
            {
                try
                {
                    #if DEBUG
                    result = (ProgramSettings)XmlFactory.LoadFromFile(typeof(ProgramSettings), Filename, new XmlFactorySettings());
                    #else
                    result = (ProgramSettings)XmlFactory.LoadFromFile(typeof(ProgramSettings), Filename, new XmlFactorySettings() { Password = _key });
#endif

                    if (result.Display.Count == 0)
                        result.ResetDisplay();

                    if (result.DisplaySystray.Count == 0)
                        result.ResetDisplaySystray();

                    return result;
                }
                catch (Exception loadException)
                {
                    MessageBox.Show(loadException.ToString());
                }
            }

            result = new ProgramSettings();
            result.ResetDisplay();
            result.ResetDisplaySystray();

            try
            {
                // Si on arrive pas à sauvegarder, un met un Guid vide. Pour ne pas avoir plein de Guid unique inutile dans les logs
                if (!result.Save())
                    result.Id = Guid.Empty;
            }
            catch { }

            return result;
        }

        public void ResetDisplay()
        {
            Display.Clear();
            Display.Add(DisplayInfoTypes.UsagePeriod);
            Display.Add(DisplayInfoTypes.Overcharge);
            Display.Add(DisplayInfoTypes.DayRemaining);
            Display.Add(DisplayInfoTypes.SuggestCombined);
            Display.Add(DisplayInfoTypes.UploadDownloadGraph);
            Display.Add(DisplayInfoTypes.CombinedGraph);
            Display.Add(DisplayInfoTypes.HistoryGraph);
        }

        public void ResetDisplaySystray()
        {
            DisplaySystray.Clear();
            DisplaySystray.Add(DisplayInfoTypes.CombinedGraph);
        }

        public void ResetColor()
        {
            UploadColor = new ColorRGB(8, 141, 182); Notify("UploadColor");
            DownloadColor = new ColorRGB(221, 167, 15); Notify("DownloadColor");
            CombinedColor = new ColorRGB(0, 100, 50); Notify("CombinedColor");
        }
        /*public void Anonymiser()
        {
            EmailSMTP.Password = String.Empty;
            EmailSMTP.Username = String.Empty;
            foreach (CIVAccount account in Accounts)
                account.Account.Username = "VLXXXXXX";
        }*/

        public static string Filename
        {
            get { return Path.Combine(CIV.Common.IO.GetCivDataFolder(), _filename); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}