using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Videotron;
using CIV.Common;

namespace CIV
{
    public class CIVAccount : INotifyPropertyChanged
    {
        public Guid Id { get; set; }

        private VideotronAccount _account;

        public VideotronAccount Account
        {
            get { return _account; }
            set { _account = value; Notify("Account"); }
        }

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; Notify("IsActive"); }
        }

        private Alert _alertSettings;

        public Alert AlertSettings
        {
            get { return _alertSettings; }
            set { _alertSettings = value; Notify("AlertSettings"); }
        }

        private bool _sendMail;

        public bool SendMail
        {
            get { return _sendMail; }
            set { _sendMail = value; Notify("SendMail"); }
        }

        private string _mailSubject;

        public string MailSubject
        {
            get { return _mailSubject; }
            set { _mailSubject = value; Notify("MailSubject"); }
        }

        private string _mailRecipients;

        public string MailRecipients
        {
            get { return _mailRecipients; }
            set { _mailRecipients = value; Notify("MailRecipients"); }
        }

        private string _mailTemplate;

        public string MailTemplate
        {
            get { return _mailTemplate; }
            set { _mailTemplate = value; Notify("MailTemplate"); }
        }

        private bool _systrayDisplay;

        public bool SystrayDisplay
        {
            get { return _systrayDisplay; }
            set { _systrayDisplay = value; Notify("SystrayDisplay"); }
        }

        public bool AlertAchieved
        {
            get
            {
                if (!AlertSettings.IsActive)
                    return false;

                double quantityLimit;

                if (AlertSettings.AlertType == AlertTypes.Quota)
                {
                    switch (AlertSettings.QuotaUnit)
                    {
                        case SIUnitTypes.Mo: quantityLimit = AlertSettings.QuotaQuantity * 1024; break;
                        case SIUnitTypes.Go: quantityLimit = AlertSettings.QuotaQuantity * 1048576; break;
                        default: quantityLimit = AlertSettings.QuotaQuantity; break;
                    }

                    return Account.Combined >= quantityLimit;
                }
                else
                {
                    return Account.CombinedPercent >= AlertSettings.PercentageQuantity;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public CIVAccount()
        {
            IsActive = true;
            AlertSettings = new Alert();
            Account = new VideotronAccount();
            SendMail = false;
            Id = Guid.NewGuid();
            SystrayDisplay = false;
        }
    }
}
