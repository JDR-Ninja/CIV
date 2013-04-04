using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms.Integration;
using CIV.Common;

namespace CIV
{
    internal class DisplayInfoFactory
    {
        private static CIVResourceManager ressource = new CIVResourceManager();

        public static UIElement Create(DisplayInfoTypes displayInfoTypes, params object[] param)
        {
            StackPanel panel;
            TextBlock label;
            TextBlock data;
            Binding binding;

            switch (displayInfoTypes)
            {
                case DisplayInfoTypes.UsagePeriod:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_lblUsagePeriod");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.PeriodStart", new DateConverter(), "d MMMM");
                    panel.Children.Add(data);

                    data = createDataLabel(null, null, null);

                    binding = new Binding("Text.ClientDashboard_At");
                    binding.Mode = BindingMode.OneTime;
                    binding.Source = ressource;
                    data.SetBinding(TextBlock.TextProperty, binding);
                    data.Margin = new Thickness(4, 0, 4, 0);

                    panel.Children.Add(data);

                    data = createDataLabel("Account.PeriodEnd", new DateConverter(), "d MMMM yyyy");
                    panel.Children.Add(data);

                    return panel;

                case DisplayInfoTypes.Overcharge:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_Overcharge");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.Overcharge", new DoubleConverter(), "C");
                    panel.Children.Add(data);

                    return panel;

                case DisplayInfoTypes.DayRemaining:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_lblDayRemaining");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.DayRemaining", new TimeSpanConverter(), null);
                    panel.Children.Add(data);

                    return panel;

                case DisplayInfoTypes.Upload:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_Upload");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.Upload", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.UploadPercent:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_UploadPercent");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.UploadPercent", new DoubleConverter(), "P");
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.Download:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_Download");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.Download", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.DownloadPercent:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_DownloadPercent");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.DownloadPercent", new DoubleConverter(), "P");
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.Combined:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_Combined");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.Combined", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.CombinedPercent:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_CombinedPercent");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.CombinedPercent", new DoubleConverter(), "P");
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.CombinedOnTotal:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_CombinedOnTotal");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.Combined", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);

                    label = createLabel("Text.On");
                    label.SetResourceReference(TextBlock.StyleProperty, "DynamicData");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.CombinedMaximum", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);

                    return panel;

                case DisplayInfoTypes.AverageCombined:
                   panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_AverageCombined");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.AverageCombined", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.SuggestCombined:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_lblSuggestCombined");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.SuggestCombined", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);
                    return panel;
                case DisplayInfoTypes.SuggestCombinedPercent:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_SuggestCombinedPercent");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.SuggestCombinedPercent", new DoubleConverter(), "P");
                    panel.Children.Add(data);

                    return panel;

                case DisplayInfoTypes.EstimateCombined:
                   panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_EstimateCombined");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.EstimateCombined", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.EstimateTotalCombined:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_AtThisRate");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.EstimateTotalCombined", new UpDownConverter(), null);
                    panel.Children.Add(data);

                    label = createLabel("Text.ClientDashboard_OfYourLimit");
                    panel.Children.Add(label);

                    return panel;

                case DisplayInfoTypes.TheoryDailyCombined:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_TheoryDailyCombined");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.TheoryDailyCombined", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.TheoryDailyCombinedPercent:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_TheoryDailyCombinedPercent");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.TheoryDailyCombinedPercent", new DoubleConverter(), "P");
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.TheoryCombinedDifference:
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    label = createLabel("Text.ClientDashboard_TheoryCombinedDifference");
                    panel.Children.Add(label);

                    data = createDataLabel("Account.TheoryCombinedVersusCombined", new SIUnitStringConverter(), null);
                    panel.Children.Add(data);
                    return panel;

                case DisplayInfoTypes.UploadDownloadGraph:
                    return new UploadDownload();

                case DisplayInfoTypes.CombinedGraph:
                    return new Combined();

                case DisplayInfoTypes.HistoryGraph:
                    WindowsFormsHost host = new WindowsFormsHost();
                    host.Name = "zedHost";
                    host.Margin = new Thickness(0, 5, 0, 0);
                    host.Padding = new Thickness(6, 0, 6, 0);
                    host.Height = 230;

                    CIVAccount account = param[0] as CIVAccount;

                    host.Child = new GraphFactory().Generate(account.Account.Username,
                                                            new Period()
                                                            {
                                                                Start = account.Account.PeriodStart,
                                                                End = account.Account.PeriodEnd
                                                            },0,0);

                    return host;
            }
            return new StackPanel();
        }

        private static TextBlock createLabel(string text)
        {
            TextBlock element = new TextBlock();

            Binding binding = new Binding(text);
            binding.Mode = BindingMode.OneTime;
            binding.Source = ressource;
            element.SetBinding(TextBlock.TextProperty, binding);

            return element;
        }

        private static TextBlock createDataLabel(string bindingPath, IValueConverter converter, object converterParam)
        {
            TextBlock element = new TextBlock();
            element.SetResourceReference(TextBlock.StyleProperty, "DynamicData");

            if (!String.IsNullOrEmpty(bindingPath))
            {
                Binding binding = new Binding(bindingPath);
                binding.Mode = BindingMode.OneWay;
                binding.Converter = converter;
                binding.ConverterParameter = converterParam;
                element.SetBinding(TextBlock.TextProperty, binding);
            }

            return element;
        }
    }
}
