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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Videotron;
using CIV.Common;

namespace CIV
{
    /// <summary>
    /// Interaction logic for QuickStatsClient.xaml
    /// </summary>
    public partial class QuickStatsClient : UserControl
    {

        private CIVAccount _client;

        public CIVAccount Client
        {
            get { return _client; }
            set { _client = value; SetUI(); }
        }

        public QuickStatsClient(CIVAccount client)
        {
            InitializeComponent();
            Client = client;
        }

        private void SetUI()
        {
            DataContext = Client;

            Refresh();

            spMainContainer.Children.Clear();

            foreach (DisplayInfoTypes element in ProgramSettings.Instance.DisplaySystray)
                spMainContainer.Children.Insert(spMainContainer.Children.Count, DisplayInfoFactory.Create(element, Client));
        }

        public void Refresh()
        {
            if (Client.Account.Overcharge > 0)
            {

                imgAlert.Source = new BitmapImage(new Uri(@"pack://application:,,,/CIV;component/Images/Alert-Quota.png"));
                imgAlert.Visibility = System.Windows.Visibility.Visible;
            }
            else if (Client.AlertAchieved)
            {

                imgAlert.Source = new BitmapImage(new Uri(@"pack://application:,,,/CIV;component/Images/Alert-User.png"));
                imgAlert.Visibility = System.Windows.Visibility.Visible;
            }
            else
                imgAlert.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
