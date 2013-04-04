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

namespace CIV
{
    /// <summary>
    /// Interaction logic for QuickStats.xaml
    /// </summary>
    public partial class QuickStats : UserControl
    {

        #region Clients dependency property

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty ClientsProperty =
            DependencyProperty.Register("Clients",
                                        typeof(List<CIVAccount>),
                                        typeof(QuickStats),
                                        new FrameworkPropertyMetadata(new List<CIVAccount>()));

        /// <summary>
        /// A property wrapper for the <see cref="ClientsProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public List<CIVAccount> Clients
        {
            get { return (List<CIVAccount>)GetValue(ClientsProperty); }
            set { SetValue(ClientsProperty, value); }
        }

        #endregion

        public QuickStats()
        {
            InitializeComponent();
        }

        public void Refresh(bool regenerate)
        {
            if (regenerate)
            {
                spClients.Children.Clear();

                foreach (CIVAccount client in Clients)
                    spClients.Children.Add(new QuickStatsClient(client));
            }
            else
            {
                foreach (QuickStatsClient element in spClients.Children)
                {
                    element.Refresh();
                }
            }
        }
    }
}
