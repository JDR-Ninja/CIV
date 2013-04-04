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
using CIV.DAL;
using ZedGraph;
using System.Drawing;
using CIV.BOL;
using System.Collections.ObjectModel;
using CIV.Common;

namespace CIV
{
    /// <summary>
    /// Interaction logic for AdvancedStats.xaml
    /// </summary>
    public partial class AdvancedStats : Window
    {
        private bool _setDatePicker;

        private UsageStats _stats;

        public UsageStats Stats
        {
            get { return _stats; }
            set { _stats = value; }
        }

        private string _account;

        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }

        public ObservableCollection<BillPeriod> BillPeriodList { get; set; }

        public AdvancedStats()
        {
            _setDatePicker = false;

            InitializeComponent();

            this.Title = CIV.strings.AdvancedStats_Title;
            this.DataContext = this;

            BillPeriodList = new ObservableCollection<BillPeriod>();
        }

        public void LoadData()
        {
            if (DataBaseFactory.Instance.IsAvailable)
            {
                Stats = new UsageStats();

                // Calcul des périodes de facturation
                foreach (Period period in DailyUsageDAO.Instance.AllPeriod(Account))
                {
                    BillPeriodList.Add(new BillPeriod()
                    {
                        Start = period.Start,
                        End = period.End,
                        Text = String.Format(CIV.strings.AdvancedStats_BillingPeriod,
                                             period.Start.ToShortDateString(),
                                             period.End.ToShortDateString())
                        //period.Start.ToString("d MMMM"),
                        //period.End.ToString("d MMMM yyyy"))
                    });
                }

                // Calcul des statistiques Globale
                List<DailyUsageBO> data = DailyUsageDAO.Instance.All(Account);
                Stats.RecordedDay = data.Count;
                Stats.GlobalUploadTotal = 0;
                Stats.GlobalDownloadTotal = 0;
                Stats.GlobalCombinedTotal = 0;
                if (data.Count > 1)
                {
                    Stats.GlobalUploadGreater = data[0].Upload;
                    Stats.GlobalUploadGreaterDay = data[0].Day;
                    Stats.GlobalUploadLesser = data[0].Upload;
                    Stats.GlobalUploadLesserDay = data[0].Day;
                    Stats.GlobalDownloadGreater = data[0].Download;
                    Stats.GlobalDownloadGreaterDay = data[0].Day;
                    Stats.GlobalDownloadLesser = data[0].Download;
                    Stats.GlobalDownloadLesserDay = data[0].Day;
                    Stats.GlobalCombinedGreater = data[0].Total;
                    Stats.GlobalCombinedGreaterDay = data[0].Day;
                    Stats.GlobalCombinedLesser = data[0].Total;
                    Stats.GlobalCombinedLesserDay = data[0].Day;
                    Stats.MoreOldDay = data[0].Day;
                    Stats.MoreRecentDay = data[data.Count - 1].Day;
                }

                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].Upload > Stats.GlobalUploadGreater)
                    {
                        Stats.GlobalUploadGreater = data[i].Upload;
                        Stats.GlobalUploadGreaterDay = data[i].Day;
                    }
                    if (data[i].Upload < Stats.GlobalUploadLesser)
                    {
                        Stats.GlobalUploadLesser = data[i].Upload;
                        Stats.GlobalUploadLesserDay = data[i].Day;
                    }
                    Stats.GlobalUploadTotal += data[i].Upload;

                    if (data[i].Download > Stats.GlobalDownloadGreater)
                    {
                        Stats.GlobalDownloadGreater = data[i].Download;
                        Stats.GlobalDownloadGreaterDay = data[i].Day;
                    }
                    if (data[i].Download < Stats.GlobalDownloadLesser)
                    {
                        Stats.GlobalDownloadLesser = data[i].Download;
                        Stats.GlobalDownloadLesserDay = data[i].Day;
                    }
                    Stats.GlobalDownloadTotal += data[i].Download;

                    if (data[i].Total > Stats.GlobalCombinedGreater)
                    {
                        Stats.GlobalCombinedGreater = data[i].Total;
                        Stats.GlobalCombinedGreaterDay = data[i].Day;
                    }
                    if (data[i].Total < Stats.GlobalCombinedLesser)
                    {
                        Stats.GlobalCombinedLesser = data[i].Total;
                        Stats.GlobalCombinedLesserDay = data[i].Day;
                    }
                    Stats.GlobalCombinedTotal += data[i].Total;
                }

                // Calcul des statistiques Mensuel
                data = DailyUsageDAO.Instance.AllByMonth(Account);
                Stats.RecordedMonth = data.Count;
                if (data.Count > 1)
                {
                    Stats.MonthlyUploadGreater = data[0].Upload;
                    Stats.MonthlyUploadGreaterDay = data[0].Day;
                    Stats.MonthlyUploadLesser = data[0].Upload;
                    Stats.MonthlyUploadLesserDay = data[0].Day;
                    Stats.MonthlyDownloadGreater = data[0].Download;
                    Stats.MonthlyDownloadGreaterDay = data[0].Day;
                    Stats.MonthlyDownloadLesser = data[0].Download;
                    Stats.MonthlyDownloadLesserDay = data[0].Day;
                    Stats.MonthlyCombinedGreater = data[0].Total;
                    Stats.MonthlyCombinedGreaterDay = data[0].Day;
                    Stats.MonthlyCombinedLesser = data[0].Total;
                    Stats.MonthlyCombinedLesserDay = data[0].Day;
                }

                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].Upload > Stats.MonthlyUploadGreater)
                    {
                        Stats.MonthlyUploadGreater = data[i].Upload;
                        Stats.MonthlyUploadGreaterDay = data[i].Day;
                    }
                    if (data[i].Upload < Stats.MonthlyUploadLesser)
                    {
                        Stats.MonthlyUploadLesser = data[i].Upload;
                        Stats.MonthlyUploadLesserDay = data[i].Day;
                    }

                    if (data[i].Download > Stats.MonthlyDownloadGreater)
                    {
                        Stats.MonthlyDownloadGreater = data[i].Download;
                        Stats.MonthlyDownloadGreaterDay = data[i].Day;
                    }
                    if (data[i].Download < Stats.MonthlyDownloadLesser)
                    {
                        Stats.MonthlyDownloadLesser = data[i].Download;
                        Stats.MonthlyDownloadLesserDay = data[i].Day;
                    }

                    if (data[i].Total > Stats.MonthlyCombinedGreater)
                    {
                        Stats.MonthlyCombinedGreater = data[i].Total;
                        Stats.MonthlyCombinedGreaterDay = data[i].Day;
                    }
                    if (data[i].Total < Stats.MonthlyCombinedLesser)
                    {
                        Stats.MonthlyCombinedLesser = data[i].Total;
                        Stats.MonthlyCombinedLesserDay = data[i].Day;
                    }
                }
                Stats.MonthlyUploadAverage = Stats.GlobalUploadTotal / data.Count;
                Stats.MonthlyDownloadAverage = Stats.GlobalDownloadTotal / data.Count;
                Stats.MonthlyCombinedAverage = Stats.GlobalCombinedTotal / data.Count;
            }
        }

        private void CalcStat(List<DailyUsageBO> data)
        {
            
        }

        private void SetDatePicker(DateTime start, DateTime end)
        {
            _setDatePicker = true;
            dpPeriodStart.DisplayDate = start;
            dpPeriodStart.Text = dpPeriodStart.DisplayDate.ToShortDateString();

            dpPeriodEnd.DisplayDate = end;
            dpPeriodEnd.Text = dpPeriodEnd.DisplayDate.ToShortDateString();
            _setDatePicker = false;
        }

        private string ConvertQuantityToStr(double value)
        {
            switch (ProgramSettings.Instance.ShowUnitType)
            {
                case SIUnitTypes.Mo: return String.Format("{0:N2}mo", value / 1024);
                case SIUnitTypes.Go: return String.Format("{0:N2}go", value / 1048576);
                default: return String.Format("{0:N2}ko", value);
            }
        }

        private double ConvertQuantity(double value)
        {
            switch (ProgramSettings.Instance.ShowUnitType)
            {
                case SIUnitTypes.Mo: return value / 1024;
                case SIUnitTypes.Go: return value / 1048576;
                default: return value;
            }
        }

        public void UpdateGraph(DateTime startDate, DateTime endDate)
        {
            if (DataBaseFactory.Instance.IsAvailable)
            {
                txtPeriod.Text = String.Format(CIV.strings.AdvancedStats_BillingPeriod,
                                                  startDate.ToShortDateString(),
                                                  endDate.ToShortDateString());
                //startDate.ToString("d MMMM yyyy"),
                //endDate.ToString("d MMMM yyyy"));

                List<DailyUsageBO> data = DailyUsageDAO.Instance.UsageByPeriod(Account, startDate, endDate);

                // Calcul des statistiques de la période courante
                Stats.PeriodUploadTotal = 0;
                Stats.PeriodDownloadTotal = 0;
                Stats.PeriodCombinedTotal = 0;
                if (data.Count > 1)
                {
                    Stats.PeriodUploadGreater = data[0].Upload;
                    Stats.PeriodUploadGreaterDay = data[0].Day;
                    Stats.PeriodUploadLesser = data[0].Upload;
                    Stats.PeriodUploadLesserDay = data[0].Day;
                    Stats.PeriodDownloadGreater = data[0].Download;
                    Stats.PeriodDownloadGreaterDay = data[0].Day;
                    Stats.PeriodDownloadLesser = data[0].Download;
                    Stats.PeriodDownloadLesserDay = data[0].Day;
                    Stats.PeriodCombinedGreater = data[0].Total;
                    Stats.PeriodCombinedGreaterDay = data[0].Day;
                    Stats.PeriodCombinedLesser = data[0].Total;
                    Stats.PeriodCombinedLesserDay = data[0].Day;
                }

                Stats.Details.Clear();

                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].Upload > Stats.PeriodUploadGreater)
                    {
                        Stats.PeriodUploadGreater = data[i].Upload;
                        Stats.PeriodUploadGreaterDay = data[i].Day;
                    }
                    if (data[i].Upload < Stats.PeriodUploadLesser)
                    {
                        Stats.PeriodUploadLesser = data[i].Upload;
                        Stats.PeriodUploadLesserDay = data[i].Day;
                    }
                    Stats.PeriodUploadTotal += data[i].Upload;

                    if (data[i].Download > Stats.PeriodDownloadGreater)
                    {
                        Stats.PeriodDownloadGreater = data[i].Download;
                        Stats.PeriodDownloadGreaterDay = data[i].Day;
                    }
                    if (data[i].Download < Stats.PeriodDownloadLesser)
                    {
                        Stats.PeriodDownloadLesser = data[i].Download;
                        Stats.PeriodDownloadLesserDay = data[i].Day;
                    }
                    Stats.PeriodDownloadTotal += data[i].Download;

                    if (data[i].Total > Stats.PeriodCombinedGreater)
                    {
                        Stats.PeriodCombinedGreater = data[i].Total;
                        Stats.PeriodCombinedGreaterDay = data[i].Day;
                    }
                    if (data[i].Total < Stats.PeriodCombinedLesser)
                    {
                        Stats.PeriodCombinedLesser = data[i].Total;
                        Stats.PeriodCombinedLesserDay = data[i].Day;
                    }
                    Stats.PeriodCombinedTotal += data[i].Total;

                    Stats.Details.Add(data[i]);
                }
                Stats.PeriodUploadAverage = Stats.PeriodUploadTotal / data.Count;
                Stats.PeriodDownloadAverage = Stats.PeriodDownloadTotal / data.Count;
                Stats.PeriodCombinedAverage = Stats.PeriodCombinedTotal / data.Count;

                // Graphique
                ZedGraphControl zedUsageGraph = new ZedGraphControl();
                zedUsageGraph.IsShowPointValues = true;
                zedUsageGraph.Dock = System.Windows.Forms.DockStyle.Fill;

                zedHost.Child = zedUsageGraph;

                zedUsageGraph.MasterPane.PaneList.Clear();
                zedUsageGraph.MasterPane.Add(new GraphPane(new RectangleF(0, 0, zedUsageGraph.Size.Width, zedUsageGraph.Size.Height),
                                                          String.Format(CIV.strings.AdvancedStats_BillingPeriod,
                                                                        startDate.ToShortDateString(),
                                                                        endDate.ToShortDateString()),
                    //startDate.ToString("d MMMM yyyy"),
                    //endDate.ToString("d MMMM yyyy")),
                                                          String.Empty, String.Empty
                                                          ));
                GraphPane myPane = zedUsageGraph.GraphPane;
                myPane.Legend.IsVisible = true;
                myPane.BarSettings.Type = BarType.Stack;

                myPane.XAxis.MajorTic.IsBetweenLabels = true;
                myPane.XAxis.Type = AxisType.Text;

                if (data.Count > 0)
                {
                    List<string> xLabel = new List<string>();
                    PointPairList pplDwl = new PointPairList();
                    PointPairList pplUpl = new PointPairList();
                    string toolHint = null;
                    int i;

                    for (i = 0; i < data.Count; i++)
                    {
                        xLabel.Add(data[i].Day.ToString("MM-dd"));

                        toolHint = String.Format("{0}\r\n{1} : {2}\r\n{3} : {4}",
                                                 data[i].Day.ToLongDateString(),
                                                 CIV.strings.AdvancedStats_Upload,
                                                 ConvertQuantityToStr(data[i].Upload),
                                                 CIV.strings.AdvancedStats_Download,
                                                 ConvertQuantityToStr(data[i].Download));

                        pplDwl.Add(new PointPair((double)i,
                                                 ConvertQuantity(data[i].Download),
                                                 toolHint));

                        pplUpl.Add(new PointPair((double)i,
                                                 ConvertQuantity(data[i].Upload),
                                                 toolHint));

                    }

                    BarItem myBar = myPane.AddBar(String.Empty, pplDwl, System.Drawing.Color.FromArgb(ProgramSettings.Instance.DownloadColor.Red, ProgramSettings.Instance.DownloadColor.Green, ProgramSettings.Instance.DownloadColor.Blue));
                    myBar.Bar.Fill = new Fill(System.Drawing.Color.FromArgb(ProgramSettings.Instance.DownloadColor.Red, ProgramSettings.Instance.DownloadColor.Green, ProgramSettings.Instance.DownloadColor.Blue));

                    myBar = myPane.AddBar(String.Empty, pplUpl, System.Drawing.Color.FromArgb(ProgramSettings.Instance.UploadColor.Red, ProgramSettings.Instance.UploadColor.Green, ProgramSettings.Instance.UploadColor.Blue));
                    myBar.Bar.Fill = new Fill(System.Drawing.Color.FromArgb(ProgramSettings.Instance.UploadColor.Red, ProgramSettings.Instance.UploadColor.Green, ProgramSettings.Instance.UploadColor.Blue));

                    myPane.XAxis.Scale.TextLabels = xLabel.ToArray();
                }
                myPane.Title.FontSpec.FontColor = System.Drawing.Color.White;

                myPane.XAxis.Scale.FontSpec.Angle = 90;
                myPane.XAxis.Scale.FontSpec.FontColor = System.Drawing.Color.White;
                myPane.XAxis.Scale.FontSpec.Family = "Verdana";
                myPane.YAxis.Scale.FontSpec.FontColor = System.Drawing.Color.White;
                myPane.YAxis.Scale.FontSpec.Family = "Verdana";

                myPane.Fill = new Fill(System.Drawing.Color.FromArgb(44, 44, 44));
                myPane.Chart.Fill = new Fill(System.Drawing.Color.FromArgb(44, 44, 44));

                zedUsageGraph.AxisChange();
            }
        }

        private void cbBillPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBillPeriod.SelectedValue != null)
            {
                BillPeriod period = cbBillPeriod.SelectedValue as BillPeriod;
                SetDatePicker(period.Start, period.End);
                UpdateGraph(period.Start, period.End);
            }
        }

        private void DatePicker_DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_setDatePicker)
            {
                if (dpPeriodStart.SelectedDate != null && dpPeriodEnd.SelectedDate != null)
                {
                    if (dpPeriodEnd.SelectedDate.Value.CompareTo(dpPeriodStart.SelectedDate) <= 0)
                        dpPeriodEnd.SelectedDate = dpPeriodStart.SelectedDate.Value.AddMonths(1);
                    else
                        UpdateGraph(dpPeriodStart.SelectedDate.Value, dpPeriodEnd.SelectedDate.Value);
                }
            }
        }

        private void miClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
