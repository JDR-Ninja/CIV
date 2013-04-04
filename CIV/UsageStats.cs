using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CIV.BOL;

namespace CIV
{
    public class UsageStats : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private double _periodUploadTotal;

        public double PeriodUploadTotal
        {
            get { return _periodUploadTotal; }
            set { _periodUploadTotal = value; Notify("PeriodUploadTotal"); }
        }

        private double _periodUploadAverage;

        public double PeriodUploadAverage
        {
            get { return _periodUploadAverage; }
            set { _periodUploadAverage = value; Notify("PeriodUploadAverage"); }
        }

        private double _periodUploadGreater;

        public double PeriodUploadGreater
        {
            get { return _periodUploadGreater; }
            set { _periodUploadGreater = value; Notify("PeriodUploadGreater"); }
        }

        private DateTime _periodUploadGreaterDay;

        public DateTime PeriodUploadGreaterDay
        {
            get { return _periodUploadGreaterDay; }
            set { _periodUploadGreaterDay = value; Notify("PeriodUploadGreaterDay"); }
        }

        private double _periodUploadLesser;

        public double PeriodUploadLesser
        {
            get { return _periodUploadLesser; }
            set { _periodUploadLesser = value; Notify("PeriodUploadLesser"); }
        }

        private DateTime _periodUploadLesserDay;

        public DateTime PeriodUploadLesserDay
        {
            get { return _periodUploadLesserDay; }
            set { _periodUploadLesserDay = value; Notify("PeriodUploadLesserDay"); }
        }

        private double _periodDownloadTotal;

        public double PeriodDownloadTotal
        {
            get { return _periodDownloadTotal; }
            set { _periodDownloadTotal = value; Notify("PeriodDownloadTotal"); }
        }

        private double _periodDownloadAverage;

        public double PeriodDownloadAverage
        {
            get { return _periodDownloadAverage; }
            set { _periodDownloadAverage = value; Notify("PeriodDownloadAverage"); }
        }

        private double _periodDownloadGreater;

        public double PeriodDownloadGreater
        {
            get { return _periodDownloadGreater; }
            set { _periodDownloadGreater = value; Notify("PeriodDownloadGreater"); }
        }

        private DateTime _periodDownloadGreaterDay;

        public DateTime PeriodDownloadGreaterDay
        {
            get { return _periodDownloadGreaterDay; }
            set { _periodDownloadGreaterDay = value; Notify("PeriodDownloadGreaterDay"); }
        }

        private double _periodDownloadLesser;

        public double PeriodDownloadLesser
        {
            get { return _periodDownloadLesser; }
            set { _periodDownloadLesser = value; Notify("PeriodDownloadLesser"); }
        }

        private DateTime _periodDownloadLesserDay;

        public DateTime PeriodDownloadLesserDay
        {
            get { return _periodDownloadLesserDay; }
            set { _periodDownloadLesserDay = value; Notify("PeriodDownloadLesserDay"); }
        }

        private double _periodCombinedTotal;

        public double PeriodCombinedTotal
        {
            get { return _periodCombinedTotal; }
            set { _periodCombinedTotal = value; Notify("PeriodCombinedTotal"); }
        }

        private double _periodCombinedAverage;

        public double PeriodCombinedAverage
        {
            get { return _periodCombinedAverage; }
            set { _periodCombinedAverage = value; Notify("PeriodCombinedAverage"); }
        }

        private double _periodCombinedGreater;

        public double PeriodCombinedGreater
        {
            get { return _periodCombinedGreater; }
            set { _periodCombinedGreater = value; Notify("PeriodCombinedGreater"); }
        }

        private DateTime _periodCombinedGreaterDay;

        public DateTime PeriodCombinedGreaterDay
        {
            get { return _periodCombinedGreaterDay; }
            set { _periodCombinedGreaterDay = value; Notify("PeriodCombinedGreaterDay"); }
        }

        private double _periodCombinedLesser;

        public double PeriodCombinedLesser
        {
            get { return _periodCombinedLesser; }
            set { _periodCombinedLesser = value; Notify("PeriodCombinedLesser"); }
        }

        private DateTime _periodCombinedLesserDay;

        public DateTime PeriodCombinedLesserDay
        {
            get { return _periodCombinedLesserDay; }
            set { _periodCombinedLesserDay = value; Notify("PeriodCombinedLesserDay"); }
        }

        private double _globalUploadGreater;

        public double GlobalUploadGreater
        {
            get { return _globalUploadGreater; }
            set { _globalUploadGreater = value; Notify("GlobalUploadGreater"); }
        }

        private DateTime _globalUploadGreaterDay;

        public DateTime GlobalUploadGreaterDay
        {
            get { return _globalUploadGreaterDay; }
            set { _globalUploadGreaterDay = value; Notify("GlobalUploadGreaterDay"); }
        }

        private double _globalUploadLesser;

        public double GlobalUploadLesser
        {
            get { return _globalUploadLesser; }
            set { _globalUploadLesser = value; Notify("GlobalUploadLesser"); }
        }

        private DateTime _globalUploadLesserDay;

        public DateTime GlobalUploadLesserDay
        {
            get { return _globalUploadLesserDay; }
            set { _globalUploadLesserDay = value; Notify("GlobalUploadLesserDay"); }
        }

        private double _globalUploadTotal;

        public double GlobalUploadTotal
        {
            get { return _globalUploadTotal; }
            set { _globalUploadTotal = value; Notify("GlobalUploadTotal"); }
        }

        private double _globalDownloadGreater;

        public double GlobalDownloadGreater
        {
            get { return _globalDownloadGreater; }
            set { _globalDownloadGreater = value; Notify("GlobalDownloadGreater"); }
        }

        private DateTime _globalDownloadGreaterDay;

        public DateTime GlobalDownloadGreaterDay
        {
            get { return _globalDownloadGreaterDay; }
            set { _globalDownloadGreaterDay = value; Notify("GlobalDownloadGreaterDay"); }
        }

        private double _globalDownloadLesser;

        public double GlobalDownloadLesser
        {
            get { return _globalDownloadLesser; }
            set { _globalDownloadLesser = value; Notify("GlobalDownloadLesser"); }
        }

        private DateTime _globalDownloadLesserDay;

        public DateTime GlobalDownloadLesserDay
        {
            get { return _globalDownloadLesserDay; }
            set { _globalDownloadLesserDay = value; Notify("GlobalDownloadLesserDay"); }
        }

        private double _globalDownloadTotal;

        public double GlobalDownloadTotal
        {
            get { return _globalDownloadTotal; }
            set { _globalDownloadTotal = value; Notify("GlobalDownloadTotal"); }
        }

        private double _globalCombinedGreater;

        public double GlobalCombinedGreater
        {
            get { return _globalCombinedGreater; }
            set { _globalCombinedGreater = value; Notify("GlobalCombinedGreater"); }
        }

        private DateTime _globalCombinedGreaterDay;

        public DateTime GlobalCombinedGreaterDay
        {
            get { return _globalCombinedGreaterDay; }
            set { _globalCombinedGreaterDay = value; Notify("GlobalCombinedGreaterDay"); }
        }

        private double _globalCombinedLesser;

        public double GlobalCombinedLesser
        {
            get { return _globalCombinedLesser; }
            set { _globalCombinedLesser = value; Notify("GlobalCombinedLesser"); }
        }

        private DateTime _globalCombinedLesserDay;

        public DateTime GlobalCombinedLesserDay
        {
            get { return _globalCombinedLesserDay; }
            set { _globalCombinedLesserDay = value; Notify("GlobalCombinedLesserDay"); }
        }

        private double _globalCombinedTotal;

        public double GlobalCombinedTotal
        {
            get { return _globalCombinedTotal; }
            set { _globalCombinedTotal = value; Notify("GlobalCombinedTotal"); }
        }

        private double _monthlyUploadAverage;

        public double MonthlyUploadAverage
        {
            get { return _monthlyUploadAverage; }
            set { _monthlyUploadAverage = value; Notify("MonthlyUploadAverage"); }
        }

        private double _monthlyUploadGreater;

        public double MonthlyUploadGreater
        {
            get { return _monthlyUploadGreater; }
            set { _monthlyUploadGreater = value; Notify("MonthlyUploadGreater"); }
        }

        private DateTime _monthlyUploadGreaterDay;

        public DateTime MonthlyUploadGreaterDay
        {
            get { return _monthlyUploadGreaterDay; }
            set { _monthlyUploadGreaterDay = value; Notify("MonthlyUploadGreaterDay"); }
        }

        private double _monthlyUploadLesser;

        public double MonthlyUploadLesser
        {
            get { return _monthlyUploadLesser; }
            set { _monthlyUploadLesser = value; Notify("MonthlyUploadLesser"); }
        }

        private DateTime _monthlyUploadLesserDay;

        public DateTime MonthlyUploadLesserDay
        {
            get { return _monthlyUploadLesserDay; }
            set { _monthlyUploadLesserDay = value; Notify("MonthlyUploadLesserDay"); }
        }

        private double _monthlyDownloadAverage;

        public double MonthlyDownloadAverage
        {
            get { return _monthlyDownloadAverage; }
            set { _monthlyDownloadAverage = value; Notify("MonthlyDownloadAverage"); }
        }

        private double _monthlyDownloadGreater;

        public double MonthlyDownloadGreater
        {
            get { return _monthlyDownloadGreater; }
            set { _monthlyDownloadGreater = value; Notify("MonthlyDownloadGreater"); }
        }

        private DateTime _monthlyDownloadGreaterDay;

        public DateTime MonthlyDownloadGreaterDay
        {
            get { return _monthlyDownloadGreaterDay; }
            set { _monthlyDownloadGreaterDay = value; Notify("MonthlyDownloadGreaterDay"); }
        }

        private double _monthlyDownloadLesser;

        public double MonthlyDownloadLesser
        {
            get { return _monthlyDownloadLesser; }
            set { _monthlyDownloadLesser = value; Notify("MonthlyDownloadLesser"); }
        }

        private DateTime _monthlyDownloadLesserDay;

        public DateTime MonthlyDownloadLesserDay
        {
            get { return _monthlyDownloadLesserDay; }
            set { _monthlyDownloadLesserDay = value; Notify("MonthlyDownloadLesserDay"); }
        }

        private double _monthlyCombinedAverage;

        public double MonthlyCombinedAverage
        {
            get { return _monthlyCombinedAverage; }
            set { _monthlyCombinedAverage = value; Notify("MonthlyCombinedAverage"); }
        }

        private double _monthlyCombinedGreater;

        public double MonthlyCombinedGreater
        {
            get { return _monthlyCombinedGreater; }
            set { _monthlyCombinedGreater = value; Notify("MonthlyCombinedGreater"); }
        }

        private DateTime _monthlyCombinedGreaterDay;

        public DateTime MonthlyCombinedGreaterDay
        {
            get { return _monthlyCombinedGreaterDay; }
            set { _monthlyCombinedGreaterDay = value; Notify("MonthlyCombinedGreaterDay"); }
        }

        private double _monthlyCombinedLesser;

        public double MonthlyCombinedLesser
        {
            get { return _monthlyCombinedLesser; }
            set { _monthlyCombinedLesser = value; Notify("MonthlyCombinedLesser"); }
        }

        private DateTime _monthlyCombinedLesserDay;

        public DateTime MonthlyCombinedLesserDay
        {
            get { return _monthlyCombinedLesserDay; }
            set { _monthlyCombinedLesserDay = value; Notify("MonthlyCombinedLesserDay"); }
        }

        private int _recordedDay;

        public int RecordedDay
        {
            get { return _recordedDay; }
            set { _recordedDay = value; Notify("RecordedDay"); }
        }

        private int _recordedMonth;

        public int RecordedMonth
        {
            get { return _recordedMonth; }
            set { _recordedMonth = value; Notify("RecordedMonth"); }
        }

        private DateTime _moreOldDay;

        public DateTime MoreOldDay
        {
            get { return _moreOldDay; }
            set { _moreOldDay = value; Notify("MoreOldDay"); }
        }

        private DateTime _moreRecentDay;

        public DateTime MoreRecentDay
        {
            get { return _moreRecentDay; }
            set { _moreRecentDay = value; Notify("MoreRecentDay"); }
        }

        private ObservableCollection<DailyUsageBO> _details;

        public ObservableCollection<DailyUsageBO> Details
        {
            get { return _details; }
            set { _details = value; }
        }

        public UsageStats()
        {
            Details = new ObservableCollection<DailyUsageBO>();
        }
    }
}