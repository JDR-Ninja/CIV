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

namespace CIV
{
    /// <summary>
    /// Interaction logic for StringListSelect.xaml
    /// </summary>
    public partial class StringListSelect : Window
    {
        public string Selection;

        public StringListSelect(string title, List<string> list)
        {
            InitializeComponent();
            this.Title = title;
            lbItems.ItemsSource = list;
            if (list.Count > 0)
                lbItems.SelectedIndex = 0;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private void lbItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Select();
        }

        private void Select()
        {
            if (lbItems.Items.Count > 0)
            {
                if (lbItems.SelectedIndex > -1)
                    Selection = lbItems.Items[lbItems.SelectedIndex].ToString();
                else if (lbItems.SelectedIndex == -1)
                    Selection = lbItems.Items[0].ToString();
            }

            Close();
        }
    }
}
