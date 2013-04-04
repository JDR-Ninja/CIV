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
using Microsoft.Win32;
using CIV.Common;
using System.Threading;
using System.Collections.ObjectModel;
using LogFactory;
using System.Collections;

namespace CIV
{
    /// <summary>
    /// Interaction logic for GeneralSettings.xaml
    /// </summary>
    public partial class GeneralSettings : Window
    {
        private ProgramSettings _settings;
        private SupportedLanguages defaultLanguage;
        private bool _launchAtStartup;

        public ObservableCollection<DisplayInfoTypes> DisplayInfoList { get; set; }
        public ObservableCollection<DisplayInfoTypes> DisplayInfoListSystray { get; set; }

        //ListBox dragSource = null;

        public GeneralSettings()
        {
            InitializeComponent();
            this.Title = CIV.strings.GeneralSettings_Title;

            _settings = ProgramSettings.Load();

            defaultLanguage = _settings.UserLanguage;

            DisplayInfoList = new ObservableCollection<DisplayInfoTypes>();
            DisplayInfoListSystray = new ObservableCollection<DisplayInfoTypes>();
            lbDisplayList.DataContext = DisplayInfoList;
            lbDisplayListSystray.DataContext = DisplayInfoListSystray;

            GenerateDisplayList();
            GenerateDisplaySystrayList();
            
            try
            {
                RegistryKey rkAutorun = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");

                if (rkAutorun != null)
                {
                    _settings.LaunchAtStartup = rkAutorun.GetValueNames().Contains("CIV");
                    rkAutorun.Close();
                }
                else
                    _settings.LaunchAtStartup = false;
            }
            catch (Exception e)
            {
                LogFactory.LogEngine.Instance.Add(e);
            }

            _launchAtStartup = _settings.LaunchAtStartup;

            this.DataContext = _settings;

            // Windows XP et moins
            if (Environment.OSVersion.Version.Major < 6)
                tabSystray.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void GenerateDisplayList()
        {
            DisplayInfoList.Clear();
            foreach (DisplayInfoTypes element in Enum.GetValues(typeof(DisplayInfoTypes)))
                if (!_settings.Display.Exists(x => x == element))
                    DisplayInfoList.Add(element);
        }

        private void GenerateDisplaySystrayList()
        {
            DisplayInfoListSystray.Clear();
            foreach (DisplayInfoTypes element in Enum.GetValues(typeof(DisplayInfoTypes)))
                if (!_settings.DisplaySystray.Exists(x => x == element))
                    DisplayInfoListSystray.Add(element);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_settings.Display.Count == 0)
                _settings.ResetDisplay();

            if (_settings.DisplaySystray.Count == 0)
                _settings.ResetDisplaySystray();

            _settings.Save();

            if (_settings.LaunchAtStartup != _launchAtStartup)
            {
                try
                {
                    RegistryKey rkAutorun = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                    if (rkAutorun != null)
                    {
                        if (_settings.LaunchAtStartup)
                            rkAutorun.SetValue("CIV", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CIV.exe"), RegistryValueKind.String);
                        else
                            rkAutorun.DeleteValue("CIV", false);
                        rkAutorun.Close();
                    }
                }
                catch (Exception regError)
                {
                    if (regError is UnauthorizedAccessException)
                        MessageBox.Show(CIV.strings.GeneralSettings_LaunchAtStartupUnauthorized);
                    else
                        LogFactory.LogEngine.Instance.Add(regError);
                }
            }

            if (defaultLanguage != _settings.UserLanguage &&
                MessageBox.Show(CIV.strings.GeneralSettings_LangChanged, CIV.strings.GeneralSettings_Confirm, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                App.Restart();

            this.DialogResult = true;

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDisplayUploadColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog diag = new System.Windows.Forms.ColorDialog();
            
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                _settings.UploadColor = new ColorRGB(diag.Color.R, diag.Color.G, diag.Color.B);
        }

        private void btnDisplayDownloadColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog diag = new System.Windows.Forms.ColorDialog();

            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                _settings.DownloadColor = new ColorRGB(diag.Color.R, diag.Color.G, diag.Color.B);
        }

        private void btnDisplayCombinedColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog diag = new System.Windows.Forms.ColorDialog();

            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                _settings.CombinedColor = new ColorRGB(diag.Color.R, diag.Color.G, diag.Color.B);
        }

        private void btnAddElements_Click(object sender, RoutedEventArgs e)
        {
            if (lbDisplayList.SelectedItem != null)
            {
                DisplayInfoTypes element = StringToDisplayInfo((string)lbDisplayList.SelectedItem);
                _settings.Display.Add(element);

                int index = lbDisplayList.SelectedIndex;
                DisplayInfoList.Remove(element);

                lblDisplayElement.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
                lbDisplayList.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();

                lblDisplayElement.SelectedIndex = lblDisplayElement.Items.Count - 1;

                if (lbDisplayList.Items.Count > 0)
                {
                    if (index < lbDisplayList.Items.Count)
                        lbDisplayList.SelectedIndex = index;
                    else
                        lbDisplayList.SelectedIndex = --index;
                }
            }
        }

        private void btnAddElementsSystray_Click(object sender, RoutedEventArgs e)
        {
            if (lbDisplayListSystray.SelectedItem != null)
            {
                DisplayInfoTypes element = StringToDisplayInfo((string)lbDisplayListSystray.SelectedItem);
                _settings.DisplaySystray.Add(element);

                int index = lbDisplayListSystray.SelectedIndex;
                DisplayInfoListSystray.Remove(element);

                lblDisplayElementSystray.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
                lbDisplayListSystray.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();

                lblDisplayElementSystray.SelectedIndex = lblDisplayElementSystray.Items.Count - 1;

                if (lbDisplayListSystray.Items.Count > 0)
                {
                    if (index < lbDisplayListSystray.Items.Count)
                        lbDisplayListSystray.SelectedIndex = index;
                    else
                        lbDisplayListSystray.SelectedIndex = --index;
                }
            }
        }

        private void btnDeleteElements_Click(object sender, RoutedEventArgs e)
        {
            if (lblDisplayElement.SelectedIndex > -1)
            {
                int index = lblDisplayElement.SelectedIndex;
                _settings.Display.RemoveAt(index);

                GenerateDisplayList();
                lblDisplayElement.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
                lbDisplayList.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();


                if (lblDisplayElement.Items.Count > 0)
                {
                    if (index < lblDisplayElement.Items.Count)
                        lblDisplayElement.SelectedIndex = index;
                    else
                        lblDisplayElement.SelectedIndex = --index;
                }
            }
        }

        private void btnDeleteElementsSystray_Click(object sender, RoutedEventArgs e)
        {
            if (lblDisplayElementSystray.SelectedIndex > -1)
            {
                int index = lblDisplayElementSystray.SelectedIndex;
                _settings.DisplaySystray.RemoveAt(index);

                GenerateDisplaySystrayList();
                lblDisplayElementSystray.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
                lbDisplayListSystray.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();

                if (lblDisplayElementSystray.Items.Count > 0)
                {
                    if (index < lblDisplayElementSystray.Items.Count)
                        lblDisplayElementSystray.SelectedIndex = index;
                    else
                        lblDisplayElementSystray.SelectedIndex = --index;
                }
            }
        }

        private void btnResetElements_Click(object sender, RoutedEventArgs e)
        {
            _settings.ResetDisplay();

            GenerateDisplayList();
            lblDisplayElement.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            lbDisplayList.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
        }

        private void btnResetElementsSystray_Click(object sender, RoutedEventArgs e)
        {
            _settings.ResetDisplaySystray();

            GenerateDisplaySystrayList();
            lblDisplayElementSystray.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            lbDisplayListSystray.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
        }

        private void btnUpElement_Click(object sender, RoutedEventArgs e)
        {
            if (lblDisplayElement.SelectedIndex > 0)
            {
                int index = lblDisplayElement.SelectedIndex;

                DisplayInfoTypes element = _settings.Display[index];
                _settings.Display.RemoveAt(index);

                index--;

                _settings.Display.Insert(index, element);
                lblDisplayElement.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            }
        }

        private void btnDownElement_Click(object sender, RoutedEventArgs e)
        {
            if (lblDisplayElement.SelectedIndex < lblDisplayElement.Items.Count - 1)
            {
                int index = lblDisplayElement.SelectedIndex;

                DisplayInfoTypes element = _settings.Display[index];
                _settings.Display.RemoveAt(index);

                index++;

                _settings.Display.Insert(index, element);
                lblDisplayElement.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            }
        }

        private void btnUpElementSystray_Click(object sender, RoutedEventArgs e)
        {
            if (lblDisplayElementSystray.SelectedIndex > 0)
            {
                int index = lblDisplayElementSystray.SelectedIndex;

                DisplayInfoTypes element = _settings.DisplaySystray[index];
                _settings.DisplaySystray.RemoveAt(index);

                index--;

                _settings.DisplaySystray.Insert(index, element);
                lblDisplayElementSystray.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            }
        }

        private void btnDownElementSystray_Click(object sender, RoutedEventArgs e)
        {
            if (lblDisplayElementSystray.SelectedIndex < lblDisplayElementSystray.Items.Count - 1)
            {
                int index = lblDisplayElementSystray.SelectedIndex;

                DisplayInfoTypes element = _settings.DisplaySystray[index];
                _settings.DisplaySystray.RemoveAt(index);

                index++;

                _settings.DisplaySystray.Insert(index, element);
                lblDisplayElementSystray.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
            }
        }

        private void lblDisplayElement_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnDeleteElements_Click(sender, null);
        }

        private void lbDisplayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnAddElements_Click(sender, null);
        }

        private void lbDisplayListSystray_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnAddElementsSystray_Click(sender, null);
        }

        private void lblDisplayElementSystray_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnDeleteElementsSystray_Click(sender, null);
        }

        private static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }

        private DisplayInfoTypes StringToDisplayInfo(string value)
        {
            if (value == CIV.strings.DisplayInfoTypes_UsagePeriod)
                return DisplayInfoTypes.UsagePeriod;
            else if (value == CIV.strings.DisplayInfoTypes_Overcharge)
                return DisplayInfoTypes.Overcharge;
            else if (value == CIV.strings.DisplayInfoTypes_DayRemaining)
                return DisplayInfoTypes.DayRemaining;
            else if (value == CIV.strings.DisplayInfoTypes_Upload)
                return DisplayInfoTypes.Upload;
            else if (value == CIV.strings.DisplayInfoTypes_UploadPercent)
                return DisplayInfoTypes.UploadPercent;
            else if (value == CIV.strings.DisplayInfoTypes_Download)
                return DisplayInfoTypes.Download;
            else if (value == CIV.strings.DisplayInfoTypes_DownloadPercent)
                return DisplayInfoTypes.DownloadPercent;
            else if (value == CIV.strings.DisplayInfoTypes_Combined)
                return DisplayInfoTypes.Combined;
            else if (value == CIV.strings.DisplayInfoTypes_CombinedPercent)
                return DisplayInfoTypes.CombinedPercent;
            else if (value == CIV.strings.DisplayInfoTypes_CombinedOnTotal)
                return DisplayInfoTypes.CombinedOnTotal;
            else if (value == CIV.strings.DisplayInfoTypes_AverageCombined)
                return DisplayInfoTypes.AverageCombined;
            else if (value == CIV.strings.DisplayInfoTypes_SuggestCombined)
                return DisplayInfoTypes.SuggestCombined;
            else if (value == CIV.strings.DisplayInfoTypes_EstimateCombined)
                return DisplayInfoTypes.EstimateCombined;
            else if (value == CIV.strings.DisplayInfoTypes_EstimateTotalCombined)
                return DisplayInfoTypes.EstimateTotalCombined;
            else if (value == CIV.strings.DisplayInfoTypes_SuggestCombinedPercent)
                return DisplayInfoTypes.SuggestCombinedPercent;
            else if (value == CIV.strings.DisplayInfoTypes_TheoryDailyCombined)
                return DisplayInfoTypes.TheoryDailyCombined;
            else if (value == CIV.strings.DisplayInfoTypes_TheoryDailyCombinedPercent)
                return DisplayInfoTypes.TheoryDailyCombinedPercent;
            else if (value == CIV.strings.DisplayInfoTypes_TheoryCombinedDifference)
                return DisplayInfoTypes.TheoryCombinedDifference;
            else if (value == CIV.strings.DisplayInfoTypes_UploadDownloadGraph)
                return DisplayInfoTypes.UploadDownloadGraph;
            else if (value == CIV.strings.DisplayInfoTypes_CombinedGraph)
                return DisplayInfoTypes.CombinedGraph;
            else if (value == CIV.strings.DisplayInfoTypes_HistoryGraph)
                return DisplayInfoTypes.HistoryGraph;
            else
                return DisplayInfoTypes.HistoryGraph;
        }

        private void btnResetColors_Click(object sender, RoutedEventArgs e)
        {
            _settings.ResetColor();
        }

        private void ApplicationCommandsHelp(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlFactory.HTTP_CIV_WIKI_GENERALSETTINGS);
            }
            catch (Exception eProcess)
            {
                LogEngine.Instance.Add(eProcess, false);
                MessageBox.Show(eProcess.ToString(), "CIV", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*private void lbDisplayList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            dragSource = parent;
            object data = GetDataFromListBox(dragSource, e.GetPosition(parent));

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
        }

        private void lblDisplayElement_Drop(object sender, DragEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            object data = e.Data.GetData(typeof(string));
            ((IList)dragSource.ItemsSource).Remove(data);
            parent.Items.Add(data);
        }*/
    }
}
