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
using System.Reflection;
using CIV.Common;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;

namespace CIV
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public ObservableCollection<Component> Components { get; set; }

        public About()
        {
            InitializeComponent();

            this.DataContext = this;
            Components = new ObservableCollection<Component>();

            this.Title = CIV.strings.About_Title;

            if (Environment.Is64BitProcess)
                lblVersion.Text = String.Format("{0} (64 Bits)", App.VersionLongStr());
            else
                lblVersion.Text = String.Format("{0} (32 Bits)", App.VersionLongStr());

            string[] assemblyList = new string[] { System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Hardcodet.Wpf.TaskbarNotification.dll"),
                                                   System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZedGraph.dll"),
                                                   System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ICSharpCode.SharpZipLib.dll")};
            
            AssemblyName assemblyName;
            foreach (string item in assemblyList)
            {
                assemblyName = AssemblyName.GetAssemblyName(item);
                Components.Add(new Component() { Name = assemblyName.Name, Version = assemblyName.Version.ToString() });
            }

            Assembly sqlAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(p => p.FullName.Substring(0, 23) == "System.Data.SqlServerCe");
            if (sqlAssembly != null)
            {
                assemblyName = new AssemblyName(sqlAssembly.FullName);
                Components.Add(new Component() { Name = assemblyName.Name, Version = assemblyName.Version.ToString() });
            }

            ThreadStart start = delegate()
            {
                try
                {
                    CIV.CentralCenterClient.CentralCenterServiceReference.CentralCenterSoapClient client = new CIV.CentralCenterClient.CentralCenterServiceReference.CentralCenterSoapClient();
                    string version = client.RunningVersion();
                    this.Dispatch(p => p.lblCentralCenter.Text = version);
                }
                catch
                {

                }
            };
            
            new Thread(start).Start();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lblWebsite_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(UrlFactory.HTTP_CIV);
        }

        private void lblFacebook_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(UrlFactory.HTTP_FACEBOOK);
        }

        private void lblTwitter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(UrlFactory.HTTP_TWITTER);
        }
    }
}
