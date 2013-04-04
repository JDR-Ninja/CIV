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
using System.Windows.Shapes;

namespace CIV
{
    /// <summary>
    /// Interaction logic for ClientDashboardMessages.xaml
    /// </summary>
    public partial class ClientDashboardEvents : Window
    {
        private ObservableCollection<ScreenMessage> _messages;

        public ObservableCollection<ScreenMessage> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public ClientDashboardEvents(ObservableCollection<ScreenMessage> messages, string account)
        {
            InitializeComponent();
            this.DataContext = this;

            Messages = messages;

            Title = String.Format(CIV.strings.ClientDashboardEvents_Title, account);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
