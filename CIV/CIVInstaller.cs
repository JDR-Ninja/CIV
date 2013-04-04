using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows;
using System.IO;
using System.Reflection;
using CIV.Common;
using System.ServiceModel;
using CIV.CentralCenterClient.CentralCenterServiceReference;
using System.Security.Principal;
using System.Security.AccessControl;


namespace CIV
{
    [RunInstaller(true)]
    public partial class CIVInstaller : System.Configuration.Install.Installer
    {
        public CIVInstaller()
        {
            InitializeComponent();
        }

        private void SetFullControl(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    FileSecurity secur = File.GetAccessControl(filename);

                    SecurityIdentifier networkService = new SecurityIdentifier("S-1-1-0"); // Tout le monde
                    IdentityReference networkServiceIdentity = networkService.Translate(typeof(NTAccount));

                    secur.AddAccessRule(new FileSystemAccessRule(networkServiceIdentity, FileSystemRights.FullControl, AccessControlType.Allow));


                    File.SetAccessControl(filename, secur);
                }
                catch { }
            }
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            //EndpointAddress endpoint = new EndpointAddress("http://codexmundus.net/CentralCenter_v3_6_0/CentralCenter.asmx");
            //BasicHttpBinding binding = new BasicHttpBinding();
            //binding.ReaderQuotas.MaxStringContentLength = 2000000;
            //binding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            //binding.Security.Mode = BasicHttpSecurityMode.None;

            //try
            //{
            //    using (CentralCenterSoapClient centralCenter = new CentralCenterSoapClient(binding, endpoint))
            //    {
            //        centralCenter.NewInstallation(ProgramSettings.Instance.Id, App.VersionLongStr());
            //    }
            //    ProgramSettings.Instance.AppUpdateAvailable = false;
            //    ProgramSettings.Instance.AppUpdateVersion = null;
            //    ProgramSettings.Instance.Save();
            //}
            //catch { }

            //try
            //{
            //    // Mise à jours des accès internet
            //    InternetAccessUpdater internetAccessManager = new InternetAccessUpdater();
            //    internetAccessManager.Execute(binding, endpoint);
            //}
            //catch { }

            SetFullControl(ProgramSettings.Filename);
            SetFullControl(InternetAccesList.Filename);
            SetFullControl(Path.Combine(CIV.Common.IO.GetCivDataFolder(), "civ.log"));


            try
            {
                string appPath = Context.Parameters["targetPath"];

                if (File.Exists(System.IO.Path.Combine(appPath, "sqlceca35.dll"))) File.Delete(System.IO.Path.Combine(appPath, "sqlceca35.dll"));
                if (File.Exists(System.IO.Path.Combine(appPath, "sqlcecompact35.dll"))) File.Delete(System.IO.Path.Combine(appPath, "sqlcecompact35.dll"));
                if (File.Exists(System.IO.Path.Combine(appPath, "sqlceer35EN.dll"))) File.Delete(System.IO.Path.Combine(appPath, "sqlceer35EN.dll"));
                if (File.Exists(System.IO.Path.Combine(appPath, "sqlceme35.dll"))) File.Delete(System.IO.Path.Combine(appPath, "sqlceme35.dll"));
                if (File.Exists(System.IO.Path.Combine(appPath, "sqlceoledb35.dll"))) File.Delete(System.IO.Path.Combine(appPath, "sqlceoledb35.dll"));
                if (File.Exists(System.IO.Path.Combine(appPath, "sqlceqp35.dll"))) File.Delete(System.IO.Path.Combine(appPath, "sqlceqp35.dll"));
                if (File.Exists(System.IO.Path.Combine(appPath, "sqlcese35.dll"))) File.Delete(System.IO.Path.Combine(appPath, "sqlcese35.dll"));
                if (File.Exists(System.IO.Path.Combine(appPath, "System.Data.SqlServerCe.dll"))) File.Delete(System.IO.Path.Combine(appPath, "System.Data.SqlServerCe.dll"));
            }
            catch { }
        }

        public override void Uninstall(IDictionary savedState)
        {
            if (MessageBox.Show("Voulez-vous effacer vos fichiers de configuration?",
                               "CIV",
                               MessageBoxButton.YesNo,
                               MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Directory.Delete(CIV.Common.IO.GetCivDataFolder(), true);
                }
                catch { }
            }

            base.Uninstall(savedState);
        }
    }
}
