using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.Common;
using CIV.BOL;
using CIV.DAL;
using ZedGraph;
using System.Drawing;

namespace CIV
{
    public class GraphFactory
    {
        public ZedGraphControl Generate(string username, Period period, int width, int height)
        {
            ZedGraphControl zedUsageGraph = new ZedGraphControl();
            if (width != 0 && height != 0)
            {
                zedUsageGraph.Width = width;
                zedUsageGraph.Height = height;
            }

            zedUsageGraph.IsShowPointValues = true;
            zedUsageGraph.Dock = System.Windows.Forms.DockStyle.Fill;

            zedUsageGraph.MasterPane.PaneList.Clear();
            zedUsageGraph.MasterPane.Add(new GraphPane(new RectangleF(0, 0, zedUsageGraph.Size.Width, zedUsageGraph.Size.Height), "", "", ""));
            GraphPane myPane = zedUsageGraph.GraphPane;
            myPane.Legend.IsVisible = false;
            myPane.BarSettings.Type = BarType.Stack;
            myPane.XAxis.MajorTic.IsBetweenLabels = true;
            myPane.XAxis.Type = AxisType.Text;
            if (DataBaseFactory.Instance.IsAvailable)
            {
                List<DailyUsageBO> data = DailyUsageDAO.Instance.UsageByPeriod(username, period.Start, period.End);
                
                if (data.Count > 0)
                {
                    List<string> xLabel = new List<string>();
                    PointPairList pplDwl = new PointPairList();
                    PointPairList pplUpl = new PointPairList();
                    string toolHint = null;
                    int i;

                    //myPane.YAxis.Title.Text = "Consommation";

                    for (i = 0; i < data.Count; i++)
                    {
                        xLabel.Add(data[i].Day.ToString("dd"));

                        toolHint = String.Format("{0}\r\n{1}: {2}\r\n{3}: {4}",
                                                 data[i].Day.ToLongDateString(),
                                                 CIV.strings.GraphFactory_Upload,
                                                 CIV.Common.UnitsConverter.SIUnitToString(data[i].Upload, ProgramSettings.Instance.ShowUnitType),
                                                 CIV.strings.GraphFactory_Download,
                                                 CIV.Common.UnitsConverter.SIUnitToString(data[i].Download, ProgramSettings.Instance.ShowUnitType));

                        pplDwl.Add(new PointPair((double)i,
                                                 CIV.Common.UnitsConverter.ConvertKo(data[i].Download, ProgramSettings.Instance.ShowUnitType),
                                                 toolHint));

                        /*toolHint = String.Format("{0}\r\n{1}\r\n{2}",
                                                 CIV.strings.Upload,
                                                 data[i].Day.ToString("dddd, dd MMMM yyyy"),
                                                 ConvertQuantityToStr(data[i].Upload));*/

                        pplUpl.Add(new PointPair((double)i,
                                                 CIV.Common.UnitsConverter.ConvertKo(data[i].Upload, ProgramSettings.Instance.ShowUnitType),
                                                 toolHint));

                    }
                    //myPane.XAxis.Title.Text = data[0].Day.ToString("d MMMM") + " au " + data[data.Count - 1].Day.ToString("d MMMM yyyy");

                    BarItem myBar = myPane.AddBar(String.Empty, pplDwl, System.Drawing.Color.FromArgb(ProgramSettings.Instance.DownloadColor.Red, ProgramSettings.Instance.DownloadColor.Green, ProgramSettings.Instance.DownloadColor.Blue));
                    //myBar.Bar.Fill = new Fill(System.Drawing.Color.Green, System.Drawing.Color.White, System.Drawing.Color.Green);
                    myBar.Bar.Fill = new Fill(System.Drawing.Color.FromArgb(ProgramSettings.Instance.DownloadColor.Red, ProgramSettings.Instance.DownloadColor.Green, ProgramSettings.Instance.DownloadColor.Blue));

                    myBar = myPane.AddBar(String.Empty, pplUpl, System.Drawing.Color.FromArgb(ProgramSettings.Instance.UploadColor.Red, ProgramSettings.Instance.UploadColor.Green, ProgramSettings.Instance.UploadColor.Blue));
                    //myBar.Bar.Fill = new Fill(System.Drawing.Color.Blue, System.Drawing.Color.White, System.Drawing.Color.Blue);
                    myBar.Bar.Fill = new Fill(System.Drawing.Color.FromArgb(ProgramSettings.Instance.UploadColor.Red, ProgramSettings.Instance.UploadColor.Green, ProgramSettings.Instance.UploadColor.Blue));

                    myPane.XAxis.Scale.TextLabels = xLabel.ToArray();

                    //myPane.XAxis.Scale.FontSpec.Size = 16;
                    //myPane.XAxis.Scale.FontSpec.Family = "Tahoma";


                    //myPane.Chart.Fill = new Fill(System.Drawing.Color.FromArgb(255, 201, 7), System.Drawing.Color.White, 90F);
                    //myPane.Fill = new Fill(System.Drawing.Color.FromArgb(221, 226, 229), System.Drawing.Color.FromArgb(250, 250, 255), 90F);

                    //myPane.Fill = new Fill(System.Drawing.Color.FromArgb(116, 116, 116), System.Drawing.Color.White, 90F);
                }
            }
            //myPane.XAxis.Scale.FontSpec.Angle = 30;
            myPane.XAxis.Scale.FontSpec.FontColor = System.Drawing.Color.White;
            myPane.XAxis.Scale.FontSpec.Family = "Verdana";
            myPane.YAxis.Scale.FontSpec.FontColor = System.Drawing.Color.White;
            myPane.YAxis.Scale.FontSpec.Family = "Verdana";

            myPane.Fill = new Fill(System.Drawing.Color.FromArgb(44, 44, 44));
            myPane.Chart.Fill = new Fill(System.Drawing.Color.FromArgb(44, 44, 44));

            zedUsageGraph.AxisChange();

            return zedUsageGraph;
        }

    }
}
