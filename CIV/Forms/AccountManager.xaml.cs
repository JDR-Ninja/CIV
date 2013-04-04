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
using System.Collections.ObjectModel;
using System.IO;
using Videotron;
using CIV.Common;
using CIV.Mail;
using System.Threading;
using System.Windows.Threading;
using LogFactory;

namespace CIV
{
    /// <summary>
    /// Interaction logic for AccountManager.xaml
    /// </summary>
    public partial class AccountManager : Window
    {
        private PleaseWait ApiWaiting = new PleaseWait();
        private ProgramSettings _settings;

        public ObservableCollection<CIVAccount> ClientList { get; set; }

        public ObservableCollection<InternetAccess> AccesstList { get; set; }

        public ObservableCollection<string> MailTemplateList { get; set; }

        public AccountManager()
        {
            InitializeComponent();
            this.Title = CIV.strings.AccountManager_Title;
            this.DataContext = this;

            _settings = ProgramSettings.Load();

            RefreshInternetAccess();

            ClientList = new ObservableCollection<CIVAccount>();

            // Chargement de la liste des modèles de courriel
            MailTemplateList = new ObservableCollection<string>();
            
            MailFactory mailFactory = new MailFactory();
            for (int i = 0; i < mailFactory.MailTemplates.Count; i++)
                MailTemplateList.Add(mailFactory.MailTemplates[i].Name);

            foreach (CIVAccount account in _settings.Accounts)
                ClientList.Add(account);

            InitializeBinding(txtUsername, txtUsername.GetBindingExpression(TextBox.TextProperty).ParentBinding, "IsEnabled");
            InitializeBinding(txtQuotaQuantity, txtQuotaQuantity.GetBindingExpression(TextBox.TextProperty).ParentBinding, "IsEnabled");
            InitializeBinding(txtMailRecipients, txtMailRecipients.GetBindingExpression(TextBox.TextProperty).ParentBinding, "IsEnabled");

            chkSendMail.IsEnabled = _settings.EmailSMTP.Active;
        }

        private void InitializeBinding(Object obj, Binding binding, string propertyPath)
        {
            Binding newBinding;
            foreach (BaseValidationRule rule in binding.ValidationRules)
            {
                newBinding = new Binding();
                newBinding.Source = obj;
                newBinding.Path = new PropertyPath(propertyPath);
                rule.Active = new BooleanDependencyObject();
                BindingOperations.SetBinding(rule.Active, BooleanDependencyObject.ValueProperty, newBinding);
            }
        }

        public void RefreshInternetAccess()
        {
            if (AccesstList == null)
                AccesstList = new ObservableCollection<InternetAccess>(InternetAccesList.Load().Access);
            else
            {
                AccesstList.Clear(); 
                foreach (InternetAccess access in InternetAccesList.Load().Access)
                {
                    AccesstList.Add(access);
                }
            }

            string name;
            foreach (InternetAccess access in AccesstList)
            {
                name = CIV.strings.ResourceManager.GetString(String.Format("IA_{0}", access.Id));
                if (!String.IsNullOrEmpty(name))
                    access.Name[_settings.UserLanguage] = name;
            }

            cbInternetAccess.GetBindingExpression(ComboBox.ItemsSourceProperty).UpdateSource();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CIVAccount newAccount = new CIVAccount();            
            if (AccesstList.Count > 0)
                newAccount.Account.UserInternetAccess = AccesstList[0].Id;

            ClientList.Add(newAccount);
            lbClients.SelectedItem = newAccount;
            _settings.Accounts.Add(newAccount);

            txtUsername.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtName.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (cbInternetAccess.GetBindingExpression(ComboBox.SelectedValueProperty) != null)
                cbInternetAccess.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();

            if (txtMailRecipients.GetBindingExpression(ComboBox.SelectedValueProperty) != null)
                txtMailRecipients.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();

            txtUsername.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            _settings.Save();
            this.DialogResult = true;
            this.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbClients.SelectedItem != null)
            {
                _settings.Accounts.Remove((CIVAccount)lbClients.SelectedItem);
                ClientList.Remove((CIVAccount)lbClients.SelectedItem);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_settings.Accounts.Count == 0)
                btnAdd_Click(null, null);
        }

        private void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text))
            {
                txtName.Text = txtUsername.Text;
                txtName.SelectAll();
            }
        }

        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUsername.SelectAll();
        }

        private void rbAlertType_Click(object sender, RoutedEventArgs e)
        {
            txtQuotaQuantity.Focus();
        }

        private void CheckBox_State(object sender, RoutedEventArgs e)
        {
            txtMailRecipients.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void rbAlertType_Checked(object sender, RoutedEventArgs e)
        {
            //txtQuotaQuantity.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void rbAlertType_Unchecked(object sender, RoutedEventArgs e)
        {
            txtQuotaQuantity.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            txtQuotaQuantity.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text))
            {
                txtName.Text = txtUsername.Text;
            }
        }

        private void txtToken_GotFocus(object sender, RoutedEventArgs e)
        {
            // Retenir le valeur pour ne pas contacter l'API pour rien
            txtToken.Tag = txtToken.Text;
        }

        private void txtToken_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtToken.Text.CompareTo(txtToken.Tag.ToString()) != 0 && Validation.GetHasError(txtToken) == false)
            {
                
                if (MessageBox.Show(CIV.strings.AccountManager_TokenChanged, strings.Dashboard_Information, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    btnGetUsername_Click(sender, e);
            }
        }

        private void InitializeUsernameList(List<string> list)
        {
            if (ApiWaiting != null)
                ApiWaiting.Close();

            if (list == null || list.Count == 0)
                MessageBox.Show(CIV.strings.AccountManager_NoUserToken);
            else
            {
                StringListSelect select = new StringListSelect(CIV.strings.AccountManager_SelectUsername, list);
                try
                {
                    select.Owner = this;
                    select.ShowDialog();
                    txtUsername.Text = select.Selection;
                }
                catch { }
            }
        }

        private void btnGetUsername_Click(object sender, RoutedEventArgs e)
        {
            ApiWaiting = new PleaseWait();
            ApiWaiting.Title = strings.AccountManager_TokenValidation;
            ApiWaiting.Owner = this;
            ApiWaiting.Show();
            string token = txtToken.Text;
            ThreadStart start = delegate()
            {
                XmlClient client = new XmlClient(_settings.UserLanguage, token);
                try
                {
                    string message;
                    List<string> result = client.GetUsernameByToken(out message);

                    /*for (int i = 1; i < 100; i++)
                    {
                        result = client.GetUsernameByToken(out message);
                        if (!String.IsNullOrEmpty(message))
                        {
                            Dispatcher.Invoke(DispatcherPriority.Background,
                                        new Action<string>(TokenError),
                                        message);
                            break;
                        }
                    }*/

                    if (result.Count > 0)
                        Dispatcher.Invoke(DispatcherPriority.Background,
                                        new Action<List<string>>(InitializeUsernameList),
                                        result);
                    else
                        Dispatcher.Invoke(DispatcherPriority.Background,
                                        new Action<string>(TokenError),
                                        message);
                }
                catch
                {
                    Dispatcher.Invoke(DispatcherPriority.Background,
                                    new Action<List<string>>(InitializeUsernameList),
                                    null);
                }
            };

            new System.Threading.Thread(start).Start();
        }

        private void TokenError(string message)
        {
            if (ApiWaiting != null)
                ApiWaiting.Close();

            MessageBox.Show(message, CIV.strings.AccountManager_Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnTokenInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_TOKEN_INFO);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString());
            }
        }

        private void ApplicationCommandsHelp(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_CIV_WIKI_ACCOUNTMANAGER);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
