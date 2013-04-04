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
using LogFactory;
using CIV.Common;

namespace CIV
{
    /// <summary>
    /// Interaction logic for CIVErrorHandler.xaml
    /// </summary>
    public partial class CIVErrorHandler : Window
    {
        private Exception _appException;

        public Exception AppException
        {
            get { return _appException; }
            set
            {
                _appException = value;
                txtException.Text = AppException.GetType().ToString();
                txtMessage.Text = AppException.Message;
            }
        }

        public string Stacktrace;

        public CIVErrorHandler()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            LogElementBO element = new LogElementBO();
            element.Error = new CivException(AppException);

            LogEngine.Instance.Add(element, ProgramSettings.Instance.AutomaticSendReport);
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(AppException.ToString());
        }
    }
}
