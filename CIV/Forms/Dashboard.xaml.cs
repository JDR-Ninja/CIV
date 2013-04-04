using System;
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
using System.Windows.Shapes;
using System.Windows.Forms.Integration;
using ZedGraph;
using Videotron;
using System.Xml.Serialization;
using System.IO;
using CIV.DAL;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using LogFactory;
using CIV.Common;
using System.Data.SqlServerCe;
using System.Collections.ObjectModel;
using Hardcodet.Wpf.TaskbarNotification;

namespace CIV
{

    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private bool _isExit;
        private AccountManager _accountManagerForm;
        
        private delegate void MethodInvokerNoArg();

        private DispatcherTimer _appAutoUpdateTimer;
        private TimeSpan _appAutoUpdateInterval = new TimeSpan(20, 0, 1, 0); // 20 jours

        private DispatcherTimer _clientUpdateTimer;
        private TimeSpan _clientUpdateInterval = new TimeSpan(1, 0, 0);

        private DispatcherTimer _internetAccesUpdateTimer;
        private TimeSpan _internetAccesUpdateInterval = new TimeSpan(20, 0, 0, 0); // 20 jours

        // Pour afficher un message si aucun compte de configuré
        private bool _zeroAccounts;

        // Pour afficher un message si un des comptes actif n'a pas de Token
        private bool _emptyToken;

        private Hardcodet.Wpf.TaskbarNotification.TaskbarIcon MyNotifyIcon;

        /// <summary>
        /// S'affiche quand le service web dit que l'application n'est pas compatible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CentralCenterNotCompatible(object sender, EventArgs e)
        {
            MessageBox.Show(strings.Dashboard_WebNotCompatible, "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private PleaseWait AppUpdaterWaiting;

        public Dashboard()
        {
            _isExit = false;
            
            // Initialisation des timers
            _appAutoUpdateTimer = new DispatcherTimer();
            _appAutoUpdateTimer.Tick += AppAutoUpdateTimer_Tick;

            _clientUpdateTimer = new DispatcherTimer();
            _clientUpdateTimer.Tick += ClientUpdateTimer_Tick;

            _internetAccesUpdateTimer = new DispatcherTimer();
            _internetAccesUpdateTimer.Tick += InternetAccesUpdateTimer_Tick;

            try
            {
                switch (ProgramSettings.Instance.UserLanguage)
                {
                    case SupportedLanguages.French:
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-ca");
                        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                        break;
                    default:
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-ca");
                        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                        break;
                }

                InitializeComponent();

                this.Title = String.Format("{0} {1}", CIV.strings.Dashboard_Title, App.VersionStr());

                // Si la liste des accès internet est vide, il faut la remplir
                if (InternetAccesList.Instance.Access.Count == 0)
                    LaunchInternetAccessManager(true);

                // Mettre à jours l'interface si une mise à jours est disponible
                if (ProgramSettings.Instance.AppUpdateAvailable && ProgramSettings.Instance.AppUpdateVersion != null)
                {
                    // La mise à jours vient d'être installé
                    if (ProgramSettings.Instance.AppUpdateVersion.CompareTo(App.VersionLong()) == 0)
                    {
                        ProgramSettings.Instance.AppUpdateAvailable = false;
                        ProgramSettings.Instance.Save();
                    }
                    else
                        miNewVersionAvailable.Visibility = System.Windows.Visibility.Visible;
                }

                // Initialisation de la base de données
                try
                {
                    DataBaseFactory.Instance.CheckDatabase();
                }
                catch (SqlCeException sqlCeException)
                {
                    DataBaseFactory.Instance.IsAvailable = false;
                    miDB.IsEnabled = false;

                    if (sqlCeException.InnerException != null && sqlCeException.InnerException is DllNotFoundException)
                    {
                        if (MessageBox.Show(strings.ClientDashboard_dllNotFoundException, strings.ClientDashboard_Requirement, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            // http://www.microsoft.com/downloads/fr-fr/details.aspx?FamilyID=e497988a-c93a-404c-b161-3a0b323dce24
                            if (Environment.Is64BitProcess)
                                System.Diagnostics.Process.Start("http://civ.codexmundus.com/download/SSCERuntime_x64-ENU.msi");
                            else
                                System.Diagnostics.Process.Start("http://civ.codexmundus.com/download/SSCERuntime_x86-ENU.msi");
                            Close();
                        }
                        else
                            MessageBox.Show(strings.ClientDashboard_DBNotAvailable, "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Vérification pour savoir s'il faut afficher la fenêtre d'aide s'il n'y a pas de compte actif
                _zeroAccounts = ProgramSettings.Instance.Accounts.Count == 0;
                foreach (CIVAccount account in ProgramSettings.Instance.Accounts.Where(x => x.IsActive))
                    if (String.IsNullOrEmpty(account.Account.Token))
                    {
                        _emptyToken = true;
                        break;
                    }

                CreateSystrayIcon();

                // Initialiser les timers
                SetTimers();

                // Afficher pour une première fois les comptes
                RefreshAccount(false);
            }
            catch (Exception dashboardExc)
            {
                LogEngine.Instance.Add(dashboardExc);
            }
        }

        private void CreateSystrayIcon()
        {
            if (ProgramSettings.Instance.SystrayDisplay)
            {
                if (MyNotifyIcon == null)
                {
                    try
                    {
                        MyNotifyIcon = new TaskbarIcon()
                            {
                                Name = "MyNotifyIcon",
                                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                                Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/CIV;component/Icons/Application.ico")).Stream),
                                ContextMenu = new ContextMenu()
                            };

                        MyNotifyIcon.DoubleClickCommand = new DashboardShowCommand();
                        MyNotifyIcon.DoubleClickCommandParameter = this;

                        MyNotifyIcon.SetBinding(Hardcodet.Wpf.TaskbarNotification.TaskbarIcon.ToolTipTextProperty,
                                                new Binding("Text.Dashboard_Title") { Source = new CIVResourceManager() });

                        // Menu
                        MenuItem menu = new MenuItem()
                            {
                                Icon = new Image()
                                {
                                    Source = new BitmapImage(new Uri("pack://application:,,,/CIV;component/Images/Exit.png", UriKind.RelativeOrAbsolute)),
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                    Stretch = Stretch.None
                                }
                            };
                        menu.Click += miExit_Click;
                        menu.SetBinding(MenuItem.HeaderProperty,
                                        new Binding("Text.Dashboard_miExit") { Source = new CIVResourceManager() });


                        MyNotifyIcon.ContextMenu.Items.Add(menu);

                        // Contenu
                        Style style = new Style(typeof(ToolTip), null);
                        style.Setters.Add(new Setter(ContentControl.TemplateProperty,
                                          new ControlTemplate(typeof(ToolTip)) { VisualTree = new FrameworkElementFactory(typeof(ContentPresenter)) }));

                        ToolTip tip = new ToolTip()
                            {
                                Style = style,
                                Content = new QuickStats()
                            };

                        MyNotifyIcon.TrayToolTip = tip;

                    }
                    catch (Exception e) { LogEngine.Instance.Add(e, false); }
                }
            }
            else
            {
                if (MyNotifyIcon != null)
                {
                    try
                    {
                        MyNotifyIcon.Icon.Dispose();
                        MyNotifyIcon.Dispose();
                        MyNotifyIcon = null;
                    }
                    catch (Exception e) { LogEngine.Instance.Add(e, false); }
                }
            }
        }

        private void SetTimers()
        {
            // Accès internet
            if (ProgramSettings.Instance.NextInternetAccesUpdate <= DateTime.Now)
            {
                // La vérification est dû maintenant, attendre 1 minute
                _internetAccesUpdateTimer.Interval = new TimeSpan(0, 1, 0);
            }
            else
            {
                try
                {
                    _internetAccesUpdateTimer.Interval = ProgramSettings.Instance.NextInternetAccesUpdate.Subtract(DateTime.Now);
                }
                catch
                {
                    _internetAccesUpdateTimer.Interval = new TimeSpan(1, 0, 0); ;
                }
            }

            _internetAccesUpdateTimer.Start();

            // Informations de consommation
            if (ProgramSettings.Instance.AutomaticClientUpdate)
            {
                _clientUpdateTimer.Interval = new TimeSpan(0, 60 - DateTime.Now.Minute, 60 - DateTime.Now.Second);
                _clientUpdateTimer.Start();
            }
            else
                _clientUpdateTimer.Stop();

            // Application
            if (ProgramSettings.Instance.AppAutoUpdate)
            {
                if (ProgramSettings.Instance.NextAppAutoUpdate <= DateTime.Now)
                {
                    // La vérification est dû maintenant, attendre 2 minutes
                    _appAutoUpdateTimer.Interval = new TimeSpan(0, 2, 0);
                }
                else
                {
                    try
                    {
                        _appAutoUpdateTimer.Interval = ProgramSettings.Instance.NextAppAutoUpdate.Subtract(DateTime.Now);
                    }
                    catch
                    {
                        _appAutoUpdateTimer.Interval = new TimeSpan(1, 1, 0); ;
                    }
                }

                _appAutoUpdateTimer.Start();
            }
            else
                _appAutoUpdateTimer.Stop();
        }

        private void ClientUpdateEnd(object sender, EventArgs e)
        {
            this.Dispatch(p => p.RefreshSecondaryView(false));
        }

        private void AppAutoUpdateTimer_Tick(object sender, EventArgs e)
        {
            new LaunchAppUpdaterDelegate(LaunchAppUpdater).BeginInvoke(true, null, null);
            _appAutoUpdateTimer.Interval = _internetAccesUpdateInterval;
        }

        private void InternetAccesUpdateTimer_Tick(object sender, EventArgs e)
        {
            LaunchInternetAccessManager(false);
            _internetAccesUpdateTimer.Interval = _internetAccesUpdateInterval;
        }

        private void LaunchInternetAccessManager(bool visible)
        {
            PleaseWait ApiWaiting = null;

            if (visible)
            {
                ApiWaiting = new PleaseWait();

                ApiWaiting.Title = CIV.strings.Dashboard_InternetAccessProgress;

                try
                {
                    // Si la fenêtre principale n'a jamais été affiché, une exception va être levée
                    ApiWaiting.Owner = this;
                }
                catch
                {
                    ApiWaiting.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    ApiWaiting.Topmost = true;
                }

                ApiWaiting.Show();
            }

            ThreadStart start = delegate()
            {
                try
                {
                    InternetAccessUpdater internetAccessManager = new InternetAccessUpdater();
                    internetAccessManager.IsNotCompatibleClient = CentralCenterNotCompatible;
                    internetAccessManager.Execute(null, null);

                    Dispatcher.Invoke(DispatcherPriority.Background,
                                       new Action<bool, PleaseWait, Exception>(LaunchInternetAccessManager_callback),
                                       visible, ApiWaiting, null);
                }
                catch (Exception updateException)
                {
                    Dispatcher.Invoke(DispatcherPriority.Background,
                                        new Action<bool, PleaseWait, Exception>(LaunchInternetAccessManager_callback),
                                        visible, ApiWaiting, updateException);
                }
            };

            new System.Threading.Thread(start).Start();
        }

        private void LaunchInternetAccessManager_callback(bool visible, PleaseWait waiting, Exception updateException)
        {
            if (waiting != null)
                waiting.Close();

            ProgramSettings.Instance.NextInternetAccesUpdate = DateTime.Now.Add(_internetAccesUpdateInterval);
            ProgramSettings.Instance.Save();

            if (updateException != null)
            {
                LogEngine.Instance.Add(updateException, false);

                if (visible)
                    MessageBox.Show(updateException.Message, "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (_accountManagerForm != null)
                    _accountManagerForm.RefreshInternetAccess();

                if (ProgramSettings.Instance.Accounts.Count > 0)
                    RefreshAccount(false);
            }
        }

        private void LaunchClientUpdate()
        {
            if (wpClients != null)
            {
                if (wpClients.Children != null)
                {
                    foreach (UIElement element in wpClients.Children)
                    {
                        if (element is ClientDashboard)
                            ((ClientDashboard)element).LaunchUpdate(false);
                    }
                }
            }
        }

        private void ClientUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Réinitialiser l'interval à toute les heures
            if (_clientUpdateTimer.Interval.Hours != 1)
                _clientUpdateTimer.Interval = _clientUpdateInterval;

            LaunchClientUpdate();
        }

        private void RefreshSecondaryView(bool regenerate)
        {
            if (MyNotifyIcon != null && MyNotifyIcon.TrayToolTip != null)
            {
                bool showAlert = false;

                ToolTip tip = MyNotifyIcon.TrayToolTip as ToolTip;
                if (tip != null)
                {
                    QuickStats stats = tip.Content as QuickStats;
                    if (stats != null)
                    {
                        if (regenerate)
                        {
                            stats.Clients.Clear();

                            foreach (CIVAccount account in ProgramSettings.Instance.Accounts)
                                if (account.IsActive)
                                {
                                    stats.Clients.Add(account);
                                    if (account.AlertAchieved || account.Account.Overcharge > 0)
                                        showAlert = true;
                                }
                            stats.Refresh(regenerate);
                        }
                        else
                            stats.Refresh(regenerate);
                    }
                }

                // Changer l'icone
                if (showAlert && MyNotifyIcon != null)
                    MyNotifyIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/CIV;component/Icons/NotifyAlert.ico")).Stream);
                else
                    MyNotifyIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/CIV;component/Icons/Application.ico")).Stream);
            }
        }

        /// <summary>
        /// Recharge l'interface
        /// </summary>
        /// <param name="reload">Indique s'il faut recharger les options</param>
        public void RefreshAccount(bool reload)
        {
            bool minOneActive = false;

            if (reload)
                ProgramSettings.Reload();

            // Faire un backup de l'historique des messages
            Dictionary<Guid, object> history = new Dictionary<Guid, object>();
            
            // Si c'est le rectangle, on ne fait rien
            if (!((wpClients.Children.Count == 1) &&
                (wpClients.Children[0] as ClientDashboard) == null))
                foreach (ClientDashboard client in wpClients.Children)
                {
                    history.Add(client.Account.Id, client.Messages);
                    client.Dispose();
                }

            wpClients.Children.Clear();

            ClientDashboard newClient;

            foreach (CIVAccount account in ProgramSettings.Instance.Accounts)
                if (account.IsActive)
                {
                    minOneActive = true;

                    newClient = new ClientDashboard(account);
                    if (history.ContainsKey(newClient.Account.Id))
                        newClient.Messages = (ObservableCollection<ScreenMessage>)history[newClient.Account.Id];
                    newClient.UpdateEnd = ClientUpdateEnd;
                    wpClients.Children.Add(newClient);
                }

            // Dans le cas où il n'y a pas de compte affiché, il faut mettre quelque chose dans le WrapPanel.
            // Sinon l'affichage est miniature.
            if (!minOneActive)
                wpClients.Children.Add(new Rectangle() { Width=440, Height=530});

            RefreshSecondaryView(true);
        }

        private void miAccountManager_Click(object sender, RoutedEventArgs e)
        {
            _accountManagerForm = new AccountManager();
            _accountManagerForm.Owner = this;
            Nullable<bool> dialogResult = _accountManagerForm.ShowDialog();
            _accountManagerForm = null;
            if (dialogResult != null && dialogResult == true)
            {
                RefreshAccount(true);
            }
        }

        private void miGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            GeneralSettings winGeneralSettings = new GeneralSettings();
            winGeneralSettings.Owner = this;
            Nullable<bool> dialogResult = winGeneralSettings.ShowDialog();

            if (dialogResult != null && dialogResult == true)
            {
                ProgramSettings.Reload();

                CreateSystrayIcon();

                RefreshAccount(false);

                SetTimers();
            }
        }

        private void miDBCompact_Click(object sender, RoutedEventArgs e)
        {
            if (DataBaseFactory.Instance.IsAvailable)
            {
                if (MessageBox.Show(CIV.strings.Dashboard_DB_CompactQuery,
                                    CIV.strings.Dashboard_Confirm, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Exception compactException;

                    foreach (string tablename in DataBaseFactory.Instance.GetUserTables())
                    {
                        if (!ProgramSettings.Instance.Accounts.Exists(delegate(CIVAccount account) { return account.Account.Username == tablename; }))
                            DataBaseFactory.Instance.DropTable(tablename);
                    }

                    if (DataBaseFactory.Instance.Compact(out compactException))
                        MessageBox.Show(CIV.strings.Dashboard_DB_CompactEnd);
                    else
                    {
                        LogEngine.Instance.Add(compactException, ProgramSettings.Instance.AutomaticSendReport);
                        MessageBox.Show(String.Format("{0}\r\n\r\n{1}", compactException.GetType(), compactException.Message), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void miDBRepair_Click(object sender, RoutedEventArgs e)
        {
            if (DataBaseFactory.Instance.IsAvailable)
            {
                Exception repairException;

                if (DataBaseFactory.Instance.Repair(out repairException))
                    MessageBox.Show(CIV.strings.Dashboard_DB_RepairEnd);
                else
                {
                    LogEngine.Instance.Add(repairException, ProgramSettings.Instance.AutomaticSendReport);
                    MessageBox.Show(String.Format("{0}\r\n\r\n{1}", repairException.GetType(), repairException.Message), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void miDBImport_Click(object sender, RoutedEventArgs e)
        {
            if (DataBaseFactory.Instance.IsAvailable)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "InternetUsage"; // Default file name
                dlg.DefaultExt = ".export"; // Default file extension
                dlg.Filter = "Internet Usage (.export)|*.export"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    Exception importException;

                    DailyUsageDAOFactory import = new DailyUsageDAOFactory();
                    int count;
                    int total;
                    if (import.Import(dlg.FileName, out importException, out count, out total))
                        MessageBox.Show(String.Format(CIV.strings.Dashboard_DB_ImportEnd, count, total));
                    else
                    {
                        LogEngine.Instance.Add(importException);
                        MessageBox.Show(String.Format("{0}\r\n\r\n{1}", importException.GetType(), importException.Message), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void miDBExport_Click(object sender, RoutedEventArgs e)
        {
            if (DataBaseFactory.Instance.IsAvailable)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "InternetUsage"; // Default file name
                dlg.DefaultExt = ".export"; // Default file extension
                dlg.Filter = "Internet Usage (.export)|*.export"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    Exception exportException;

                    DailyUsageDAOFactory export = new DailyUsageDAOFactory();
                    int count;
                    if (export.Export(dlg.FileName, out exportException, out count))
                        MessageBox.Show(String.Format(CIV.strings.Dashboard_DB_ExportEnd, count));
                    else
                    {
                        LogEngine.Instance.Add(exportException);
                        MessageBox.Show(String.Format("{0}\r\n\r\n{1}", exportException.GetType(), exportException.Message), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void miDBReset_Click(object sender, RoutedEventArgs e)
        {
            if (DataBaseFactory.Instance.IsAvailable)
            {
                if (MessageBox.Show(CIV.strings.Dashboard_DB_ResetConfirm,
                                    CIV.strings.Dashboard_Confirm, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DataBaseFactory.Instance.ResetDatabase();
                    MessageBox.Show(CIV.strings.Dashboard_DB_ResetEnd);
                    RefreshAccount(true);
                }
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {

            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
            }
            else
            {
                this.ShowInTaskbar = true;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (_zeroAccounts)
            {
                // Il ne faut pas avertir à chaque affichage de la fenêtre
                _zeroAccounts = false;

                if (MessageBox.Show(CIV.strings.Dashboard_ZeroAccountPrompt, strings.Dashboard_Information, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _accountManagerForm = new AccountManager();
                    _accountManagerForm.Owner = this;
                    Nullable<bool> dialogResult = _accountManagerForm.ShowDialog();
                    _accountManagerForm = null;

                    if (dialogResult != null && dialogResult == true)
                        RefreshAccount(true);
                }
            }

            if (_emptyToken)
            {
                // Il ne faut pas avertir à chaque affichage de la fenêtre
                _emptyToken = false;
                if (MessageBox.Show(CIV.strings.Dashboard_TokenAlert, strings.Dashboard_Information, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _accountManagerForm = new AccountManager();
                    _accountManagerForm.Owner = this;
                    Nullable<bool> dialogResult = _accountManagerForm.ShowDialog();
                    _accountManagerForm = null;

                    if (dialogResult != null && dialogResult == true)
                        RefreshAccount(true);
                }
            }
        }

        private void miForum_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_FORUM);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miDonation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_CIV_DONATE);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            About winAbout = new About();
            winAbout.Owner = this;
            winAbout.ShowDialog();
        }

        private void miWebsite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_CIV);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            _isExit = true;
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ProgramSettings.Instance.LaunchMinimized)
                this.WindowState = System.Windows.WindowState.Minimized;

            if (ProgramSettings.Instance.AutomaticClientUpdate)
                LaunchClientUpdate();
        }

        private void miInternetAccessUpdate_Click(object sender, RoutedEventArgs e)
        {
            LaunchInternetAccessManager(true);
        }

        private void miHistory_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.txt"));
        }

        private void OnUpdateAvailable(object sender, UpdateAvailabledEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (MethodInvokerNoArg)delegate()
            {
                ProgramSettings.Instance.AppUpdateAvailable = true;
                ProgramSettings.Instance.AppUpdateVersion = e.Release.Number;
                ProgramSettings.Instance.Save();
                miNewVersionAvailable.Visibility = System.Windows.Visibility.Visible;

                Updater updater = new Updater();
                updater.Execute(e.Release);
            });
        }

        private void OnUpdateNotAvailable(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (MethodInvokerNoArg)delegate()
            {
                ProgramSettings.Instance.Save();
                miNewVersionAvailable.Visibility = System.Windows.Visibility.Collapsed;
                MessageBox.Show(strings.Dashboard_UpdateNotAvailable, "CIV", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            });
        }

        private void OnUpdateCompleted(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (MethodInvokerNoArg)delegate()
            {
                if (AppUpdaterWaiting != null)
                {
                    AppUpdaterWaiting.Close();
                }
            });
        }

        private void LaunchAppUpdater(bool silent)
        {
            ProgramSettings.Instance.AppUpdateAvailable = false;
            AppUpdater updater = new AppUpdater();
            updater.IsNotCompatibleClient = CentralCenterNotCompatible;
            updater.OnUpdateAvailable += OnUpdateAvailable;
            if (!silent)
            {
                updater.OnUpdateNotAvailable = OnUpdateNotAvailable;
                updater.OnCompleted = OnUpdateCompleted;
            }

            ProgramSettings.Instance.NextAppAutoUpdate = DateTime.Now.Add(_appAutoUpdateInterval);
            ProgramSettings.Instance.Save();
            updater.Execute(App.VersionLongStr());
        }

        public delegate void LaunchAppUpdaterDelegate(bool silent);

        private void miAppCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            AppUpdaterWaiting = new PleaseWait();
            AppUpdaterWaiting.Title = strings.Dashboard_CheckNewUpdate;
            AppUpdaterWaiting.Owner = this;
            AppUpdaterWaiting.Show();

            new LaunchAppUpdaterDelegate(LaunchAppUpdater).BeginInvoke(false, null, null);
        }

        private void miNewVersionAvailable_Click(object sender, RoutedEventArgs e)
        {
            miAppCheckUpdate_Click(sender, e);
        }

        private void miCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Backup backForm = new Backup();
                backForm.OnEnd = EndBackup;
                backForm.Owner = this;
                backForm.Show();
            }
            catch (Exception backupException)
            {
                MessageBox.Show(backupException.Message, "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miRestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BackupManager man = new BackupManager();
                if (man.RestoreBackup())
                {
                    MessageBox.Show(strings.Dashboard_RestoreBackupEnd, "CIV", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    App.Restart();
                }
            }
            catch (Exception backupException)
            {
                MessageBox.Show(backupException.Message, "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EndBackup(BackupEndEventArgs e)
        {
            if (e.IsCancelled)
                MessageBox.Show(strings.Dashboard_BackupCancel, "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(strings.Dashboard_BackupEnd, "CIV", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isExit || !ProgramSettings.Instance.MinimizeToTrayOnClose)
            {
                if (ProgramSettings.Instance.SaveDisplayPosition)
                {
                    ProgramSettings.Instance.DashboardPosition = new Point(this.Left, this.Top);
                    ProgramSettings.Instance.Save();
                }
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        private void miFacebook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_FACEBOOK);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miTwitter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_TWITTER);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void miHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_CIV_WIKI);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplicationCommandsHelp(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_CIV_WIKI_MAINSCREEN);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (MyNotifyIcon != null)
                {
                    MyNotifyIcon.Icon.Dispose();
                }
            }
            catch {}
        }

       
    }
}
