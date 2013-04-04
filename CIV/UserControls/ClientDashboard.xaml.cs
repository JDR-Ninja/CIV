using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Videotron;
using System.Threading;
using LogFactory;
using CIV.BOL;
using CIV.DAL;
using System.Runtime.Remoting.Messaging;
using ZedGraph;
using System.Drawing;
using System.IO;
using System.Windows.Threading;
using CIV.Common;
using System.Net;
using System.Net.Mail;
using CIV.Mail;
using Videotron.Wired;
using System.Windows.Forms.Integration;
using Videotron.Exceptions;
using Hardcodet.Wpf.TaskbarNotification;

namespace CIV
{
    /// <summary>
    /// Interaction logic for ClientDashboard.xaml
    /// </summary>
    public partial class ClientDashboard : UserControl, IDisposable
    {
        private bool _forceUsageRefresh;

        private delegate void MyDelegate();

        private TaskbarIcon MyNotifyIcon;


        private bool _isWorking;

        public bool IsWorking
        {
            get { return _isWorking; }
            set { _isWorking = value; }
        }

        private ObservableCollection<ScreenMessage> _messages;

        public ObservableCollection<ScreenMessage> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public EventHandler UpdateEnd;

        private XmlClient _apiClient;

        public XmlClient ApiClient
        {
            get { return _apiClient; }
            set { _apiClient = value; }
        }

        private Exception _browserException;

        public Exception BrowserException
        {
            get { return _browserException; }
            set { _browserException = value; }
        }

        private CIVAccount _account;

        public CIVAccount Account
        {
            get { return _account; }
            set { _account = value; UpdateUI(); }
        }

        public ClientDashboard(CIVAccount account)
        {
            InitializeComponent();

            DataContext = account;

            // Construction de l'interface
            foreach(DisplayInfoTypes element in ProgramSettings.Instance.Display)
                spMainContainer.Children.Insert(spMainContainer.Children.Count,
                                                DisplayInfoFactory.Create(element, account));

            IsWorking = false;

            Messages = new ObservableCollection<ScreenMessage>();

            ApiClient = new XmlClient(ProgramSettings.Instance.UserLanguage,
                                      account.Account.Token,
                                      account.Account.Username);

            ApiClient.OnReadDailyWiredUsage += ReadDailyWiredUsage;
            ApiClient.OnError += ApiClientError;
            ApiClient.OnDownloadData += OnDownloadData;
            ApiClient.OnDownloadDataEnd += OnDownloadDataEnd;
            ApiClient.OnDownloadHistory += OnDownloadHistory;
            ApiClient.OnDownloadHistoryEnd += OnDownloadHistoryEnd;

            // Déclenche le rafraichissement du UI
            Account = account;

            if (Account.Account.LastUpdate == DateTime.MinValue)
            {
                // Mettre un délai aléatoire
                LaunchUpdate(false);
            }
            else
                ShowLastUpdate();

            if (ProgramSettings.Instance.EmailSMTP.Active &&
                        Account.SendMail &&
                        !String.IsNullOrEmpty(Account.MailSubject) &&
                        !String.IsNullOrEmpty(Account.MailTemplate))
                btnSendMail.Visibility = System.Windows.Visibility.Visible;
            else
                btnSendMail.Visibility = System.Windows.Visibility.Collapsed;


            if (DataBaseFactory.Instance.IsAvailable)
            {
                List<DailyUsageBO> usages = DailyUsageDAO.Instance.UsageByPeriod(account.Account.Username, DateTime.Now.Date, DateTime.Now.Date);

                if (usages.Count > 0)
                {
                    Account.Account.CurrentDayUpload = usages[0].Upload;
                    Account.Account.CurrentDayDownload = usages[0].Download;
                }
            }
        }

        private void SetMessage(string title, string text, ScreenMessageType messageType)
        {
            Messages.Add(new ScreenMessage(title, text, messageType));

            imgInformation.Visibility = System.Windows.Visibility.Collapsed;

            lblStatus.Text = title;
        }

        private void SetMessage(Exception exception)
        {
            string title = String.Empty;
            string message = exception.Message;

            // Erreur Vidéotron
            if (exception is VideotronException)
            {
                VideotronException videotronException = exception as VideotronException;
                if (videotronException.Status == VideotronExceptionStatus.AuthentificationFailed)
                    title = CIV.strings.ClientDashboard_AuthentificationFailedException;
                else if (videotronException.Status == VideotronExceptionStatus.ResponseEmpty)
                    title = CIV.strings.ClientDashboard_WebResponseException;
            }

            // Erreur de décodage
            else if (exception is DecodeFailedException)
            {
                title = CIV.strings.ClientDashboard_DecodeFailedException;
                
                // Traduire le message d'erreur pour l'utilisateur
                switch ((exception as DecodeFailedException).Type)
                {
                    case DecodeFailedTypes.Download:
                        message = strings.DecodeDownload;
                        break;
                    case DecodeFailedTypes.Upload:
                        message = strings.DecodeUpload;
                        break;
                    case DecodeFailedTypes.Unit:
                        message = strings.DecodeUnit;
                        break;
                    case DecodeFailedTypes.TotalCombined:
                        message = strings.DecodeTotalCombined;
                        break;
                    case DecodeFailedTypes.ShowPeriods:
                        message = strings.DecodeShowPeriods;
                        break;
                    case DecodeFailedTypes.BillPeriods:
                        message = strings.DecodeBillPeriods;
                        break;
                    case DecodeFailedTypes.History:
                        message = strings.DecodeHistory;
                        break;
                    case DecodeFailedTypes.Unexpected:
                        message = strings.DecodeUnexpected;
                        break;
                }
            }
            else if (exception.Message.Length > 70)
                title = String.Format("{0}...", exception.Message.Substring(0, 70));
            else
                title = exception.Message;

            Messages.Add(new ScreenMessage(title,String.Format("{0}\r\n{1}", exception.GetType(), message), ScreenMessageType.Exception));

            imgInformation.Visibility = System.Windows.Visibility.Visible;

            lblStatus.Text = title;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            LaunchUpdate(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }

        public void LaunchUpdate(bool forceRefresh)
        {
            _forceUsageRefresh = forceRefresh;

            IsWorking = true;
            Account.Account.BeginEdit();
            BrowserException = null;
            btnUpdate.IsEnabled = false;
            btnAdvancedStats.IsEnabled = false;
            btnSendMail.IsEnabled = false;

            // Si la BD n'existe pas, on la crée
            if (DataBaseFactory.Instance.IsAvailable && !DataBaseFactory.Instance.TableExist(Account.Account.Username))
                DailyUsageDAO.Instance.CreateTable(Account.Account.Username);
            
            MyDelegate ad = new MyDelegate(UpdateClient);
            ad.BeginInvoke(new AsyncCallback(UpdateClientComplete), null);
        }

        public void UpdateClientComplete(IAsyncResult a)
        {
            _forceUsageRefresh = false;

            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
            {
                IsWorking = false;

                Account.Account.LastUpdate = DateTime.Now;

                if (_apiClient.Success)
                {
                    ProgramSettings.Instance.Save();

                    if (Account.Account.NewDataAvailable)
                        SendMail();

                    ShowLastUpdate();
                }

                Account.Account.EndEdit();

                if (UpdateEnd != null)
                    UpdateEnd(this, null);

                btnUpdate.IsEnabled = true;
                btnAdvancedStats.IsEnabled = true;
                btnSendMail.IsEnabled = true;
            }));
        }

        private void ShowLastUpdate()
        {
            TimeSpan delay = DateTime.Now.Subtract(Account.Account.LastUpdate);
            if (delay.Days <= 1)
                SetMessage(String.Format("{0} {1}, {2}",
                                         CIV.strings.ClientDashboard_LastUpdate,
                                         Account.Account.LastUpdate.ToLongDateString(),
                                         Account.Account.LastUpdate.ToLongTimeString()), String.Empty, ScreenMessageType.Normal);
            else
                SetMessage(String.Format(CIV.strings.ClientDashboard_LastUpdateWithDelay,
                                         Account.Account.LastUpdate.ToLongDateString() + ", " + Account.Account.LastUpdate.ToLongTimeString(),
                                         delay.Days), String.Empty, ScreenMessageType.Normal);
        }

        private void UpdateClient()
        {
            ApiClient.Execute();
        }

        private void ReadDailyWiredUsage(string username, WiredDailyUsage usage, ref bool stop)
        {
            // Mettre à jours les données quotidienne
            if (usage.Day.Date == DateTime.Now.Date)
            {
                Account.Account.CurrentDayUpload = usage.Upload;
                Account.Account.CurrentDayDownload = usage.Download;
            }

            if (DataBaseFactory.Instance.IsAvailable)
            {
                DailyUsageBO dailyUsage = new DailyUsageBO()
                {
                    Day = usage.Day,
                    Download = usage.Download,
                    Month = usage.Month,
                    Total = usage.Total,
                    Upload = usage.Upload,
                    Period = usage.Period
                };

                // Vérifier si le jours existe déjà dans la BD
                DailyUsageBO data;
                if (DailyUsageDAO.Instance.Exist(username, dailyUsage, out data))
                {
                    // Vérifier si les données sont différente
                    if (dailyUsage.Download != data.Download || dailyUsage.Upload != data.Upload || dailyUsage.Total != data.Total)
                        DailyUsageDAO.Instance.Update(username, dailyUsage);
                    else if (!_forceUsageRefresh)
                        stop = true;
                }
                else
                    DailyUsageDAO.Instance.Insert(username, dailyUsage);
            }
        }

        private void ApiClientError(Exception exception)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
            {
                if (!(exception is WebException) && !(exception is ApiException))
                    LogEngine.Instance.Add(new LogElementBO()
                    {
                        Error = new CivException(exception),
                        Owner = ProgramSettings.Instance.Id
                    }, ProgramSettings.Instance.AutomaticSendReport);

                SetMessage(exception);
            }));
        }

        private void OnDownloadData(Object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetMessage(CIV.strings.ClientDashboard_sbDownloadData, String.Empty, ScreenMessageType.Normal);
            }));
        }

        private void OnDownloadDataEnd(WiredAccount wiredAccount)
        {
            if (wiredAccount != null)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
                {
                    Account.Account.PeriodStart = wiredAccount.PeriodStart;
                    Account.Account.PeriodEnd = wiredAccount.PeriodEnd.AddDays(1).AddMinutes(-1);
                    Account.Account.Download = wiredAccount.DownloadedBytes;
                    Account.Account.Upload = wiredAccount.UploadedBytes;
                    Account.Account.CombinedMaximum = wiredAccount.MaxCombinedBytes;
                    //Account.Account.Combined = wiredAccount.c
                    //Account.Account.CombinedPercent = wiredAccount.CombinedPercent;

                    UpdateUI();
                    Account.Account.Refresh();
                }));
            }
        }

        private void OnDownloadHistory(Object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetMessage(CIV.strings.ClientDashboard_sbDownloadHistory, String.Empty, ScreenMessageType.Normal);
            }));
        }

        private void OnDownloadHistoryEnd(Object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
            {
                for (int i = 0; i < spMainContainer.Children.Count; i++)
                {
                    if (spMainContainer.Children[i] is WindowsFormsHost)
                    {
                        spMainContainer.Children.RemoveAt(i);
                        spMainContainer.Children.Insert(i, DisplayInfoFactory.Create(DisplayInfoTypes.HistoryGraph, Account));
                    }
                }
            }));
        }

        private void UpdateUI()
        {
            if (Account.Account.Overcharge > 0)
            {
                imgAlert.Source = new BitmapImage(new Uri(@"pack://application:,,,/CIV;component/Images/Alert-Quota.png"));
                imgAlert.Visibility = System.Windows.Visibility.Visible;
            }
            else if (Account.AlertAchieved)
            {

                imgAlert.Source = new BitmapImage(new Uri(@"pack://application:,,,/CIV;component/Images/Alert-User.png"));
                imgAlert.Visibility = System.Windows.Visibility.Visible;
            }
            else
                imgAlert.Visibility = System.Windows.Visibility.Collapsed;

            // Gestion de l'icône dans la barre système
            try
            {
                if (_account.SystrayDisplay)
                {
                    if (MyNotifyIcon == null)
                    {

                        MyNotifyIcon = new TaskbarIcon()
                        {
                            Name = "MyNotifyIcon",
                            VerticalAlignment = System.Windows.VerticalAlignment.Top,
                            ContextMenu = new ContextMenu()
                        };


                        MyNotifyIcon.DoubleClickCommand = new DashboardShowCommand();
                        MyNotifyIcon.DoubleClickCommandParameter = this.Parent;

                        MyNotifyIcon.SetBinding(Hardcodet.Wpf.TaskbarNotification.TaskbarIcon.ToolTipTextProperty,
                                                new Binding("Account.DisplayName") { Source = _account });

                        // Menu
                        MenuItem menu = new MenuItem()
                        {
                            Icon = new System.Windows.Controls.Image()
                            {
                                Source = new BitmapImage(new Uri("pack://application:,,,/CIV;component/Images/Exit.png", UriKind.RelativeOrAbsolute)),
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                Stretch = Stretch.None
                            }
                        };
                    }
                    else
                        MyNotifyIcon.Icon.Dispose();


                    UsagePieChart pie = new UsagePieChart(_account.Account, ProgramSettings.Instance.CombinedColor.GetColor(), System.Drawing.Color.White);
                    MyNotifyIcon.Icon = pie.GenerateIcon();
                }
                else
                {
                    if (MyNotifyIcon != null)
                    {                
                        MyNotifyIcon.Icon.Dispose();
                        MyNotifyIcon.Dispose();
                    }
                }
            }
            catch (Exception e) { LogEngine.Instance.Add(e, false); }

        }

        private void imgInformation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Messages.Count > 0)
                MessageBox.Show(Messages[Messages.Count - 1].Text);
        }

        private void imgReport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new ClientDashboardEvents(Messages, Account.Account.DisplayName).Show();
        }

        private void btnAdvancedStats_Click(object sender, RoutedEventArgs e)
        {
            if (!DataBaseFactory.Instance.IsAvailable || DailyUsageDAO.Instance.All(Account.Account.Username).Count == 0)
                MessageBox.Show(strings.ClientDashboard_StaticNotAvailable);
            else
            {
                AdvancedStats advancedStats = new AdvancedStats();
                advancedStats.Account = Account.Account.Username;
                advancedStats.LoadData();
                advancedStats.Show();
            }
        }

        private void btnSendMail_Click(object sender, RoutedEventArgs e)
        {
            btnSendMail.IsEnabled = false;
            SendMail();
            btnSendMail.IsEnabled = true;
        }

        /// <summary>
        /// Envoi un courriel
        /// </summary>
        private void SendMail()
        {
            if (ProgramSettings.Instance.EmailSMTP.Active &&
                        Account.SendMail &&
                        !String.IsNullOrEmpty(Account.MailRecipients) &&
                        !String.IsNullOrEmpty(Account.MailSubject) &&
                        !String.IsNullOrEmpty(Account.MailTemplate))
            //if (true)
            {
                // Validation que toutes les informations sont disponible pour l'envoi (faille de la v3.0.1)
                try
                {
                    SetMessage(CIV.strings.ClientDashboard_MailSending, String.Empty, ScreenMessageType.Normal);

                    MailFactory mailFactory = new MailFactory();
                    mailFactory.SendMailHTML(new MailTagFormater(Account.Account),
                                             ProgramSettings.Instance.EmailSMTP,
                                             Account.MailRecipients,
                                             Account.MailSubject,
                                             Account.MailTemplate,
                                             Account.Account);
                    SetMessage(CIV.strings.ClientDashboard_MailSended, Account.MailRecipients, ScreenMessageType.Normal);
                }
                catch (SmtpException smtpException)
                {
                    // Ne pas logguer les erreurs smtp
                    SetMessage(smtpException);
                }
                catch (Exception mailException)
                {
                    LogEngine.Instance.Add(new LogElementBO() { Error = new CivException(mailException) }, ProgramSettings.Instance.AutomaticSendReport);
                    SetMessage(mailException);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (MyNotifyIcon != null)
            {
                if (MyNotifyIcon.Icon != null)
                    MyNotifyIcon.Icon.Dispose();
                MyNotifyIcon.Dispose();
            }
        }

        #endregion
    }
}
