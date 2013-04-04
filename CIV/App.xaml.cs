//#define PORTABLE
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using LogFactory;
using System.Reflection;
using System.Threading;
using CIV.Common;

namespace CIV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#if DEBUG
        public const string MutexGuid = "5aa91800-fb2f-4cee-b77c-1e57354ee3cb";
#else
        public const string MutexGuid = "5aa91800-fb2f-4cee-b77c-1e57354ee3ca";
#endif
        private static Mutex _mutex;
        private static bool _isRestarting;
        private static bool _isNotTheOne;

        public Guid Owner;

        public App () : base()
        {
	        try
	        {
                _mutex = Mutex.OpenExisting(MutexGuid);
                _isNotTheOne = true;
		        Application.Current.Shutdown();
		        return;
	        }
	        catch (WaitHandleCannotBeOpenedException)
	        {
                _mutex = new Mutex(true, MutexGuid);
	        }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        /// <summary>
        /// Chargement des assemblies dynamiquement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Pour contrer ArgumentNotNullException
            int pos = String.IsNullOrEmpty(args.Name) ? -1 : args.Name.IndexOf(",");

            string filename = String.Format("{0}.dll", pos > -1 ? args.Name.Substring(0, pos) : args.Name);

            string path = AppDomain.CurrentDomain.BaseDirectory;

            // Tentative de chargement dans le répertoire de l'application
            if (System.IO.File.Exists(System.IO.Path.Combine(path, filename)))
                return Assembly.LoadFile(System.IO.Path.Combine(path, filename));

            // Redirection vers les répertoires 32bits et 64bits
            else
            {
                path = System.IO.Path.Combine(path, Environment.Is64BitProcess ? "x64\\" : "x86\\");

                if (System.IO.File.Exists(System.IO.Path.Combine(path, filename)))
                    return Assembly.LoadFile(System.IO.Path.Combine(path, filename));
            }

            // Ne pas lever d'exception, ça fait planter le programme parfois (ex: TuneUp Utility avec style Verdesh)    
            return null;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogEngine.Instance.Add(e.Exception);

            MessageBox.Show(String.Format("{0}\r\n\r\n{1}", e.Exception.GetType(), e.Exception.Message));

            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Owner = ProgramSettings.Instance.Id;
            LogFactory.LogEngine.Instance.Owner = ProgramSettings.Instance.Id;
            LogFactory.LogEngine.Instance.AutomaticSendReport = ProgramSettings.Instance.AutomaticSendReport;
            LogFactory.LogEngine.Instance.ClientVersion = VersionLongStr();

            Dashboard form = new Dashboard();

            if (ProgramSettings.Instance.SaveDisplayPosition && ProgramSettings.Instance.DashboardPosition != null)
            {
                form.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                form.Top = ProgramSettings.Instance.DashboardPosition.Y;
                form.Left = ProgramSettings.Instance.DashboardPosition.X;
            }
            else
                form.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            Application.Current.MainWindow = form;

            form.Show();
        }

        static public string VersionStr()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}", version.Major, version.Minor);
        }

        static public string VersionLongStr()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public static CIVVersion Version()
        {
            return new CIVVersion(VersionStr());
        }

        public static CIVVersion VersionLong()
        {
            return new CIVVersion(VersionLongStr());
        }

        public static void Restart()
        {
            _isRestarting = true;
            _mutex.ReleaseMutex();
            _mutex.Dispose();

            // On redémarre
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (!_isRestarting && !_isNotTheOne)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new ThreadStart(delegate
                {
                    _mutex.ReleaseMutex();
                    _mutex.Dispose();
                }));
            }
        }
    }
}
