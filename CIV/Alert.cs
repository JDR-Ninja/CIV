using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CIV.Common;

namespace CIV
{
    public class Alert : INotifyPropertyChanged
    {
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; Notify("IsActive"); }
        }

        private AlertTypes _alertType;

        public AlertTypes AlertType
        {
            get { return _alertType; }
            set { _alertType = value; Notify("AlertType"); }
        }

        private SIUnitTypes _quotaUnit;

        public SIUnitTypes QuotaUnit
        {
            get { return _quotaUnit; }
            set { _quotaUnit = value; Notify("QuotaUnit"); }
        }

        private int _quotaQuantity;

        public int QuotaQuantity
        {
            get { return _quotaQuantity; }
            set { _quotaQuantity = value; Notify("QuotaQuantity"); }
        }

        private double _percentageQuantity;

        public double PercentageQuantity
        {
            get { return _percentageQuantity; }
            set { _percentageQuantity = value; Notify("PercentageQuantity"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public Alert()
        {
            AlertType = CIV.AlertTypes.Percentage;
            QuotaQuantity = 20;
            QuotaUnit = SIUnitTypes.Go;
            PercentageQuantity = 75;
        }
    }
}
