using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class DisplayInfoTypesConverter : ConverterMarkupExtension<DisplayInfoTypesConverter>
    {
        public DisplayInfoTypesConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<string> result = new List<string>();
            if (value != null)
            {
                IEnumerable<DisplayInfoTypes> listValue = (IEnumerable<DisplayInfoTypes>)value;

                foreach (DisplayInfoTypes element in listValue)
                {
                    switch (element)
                    {
                        case DisplayInfoTypes.UsagePeriod:
                            result.Add(CIV.strings.DisplayInfoTypes_UsagePeriod); break;
                        case DisplayInfoTypes.Overcharge:
                            result.Add(CIV.strings.DisplayInfoTypes_Overcharge); break;
                        case DisplayInfoTypes.DayRemaining:
                            result.Add(CIV.strings.DisplayInfoTypes_DayRemaining); break;
                        case DisplayInfoTypes.Upload:
                            result.Add(CIV.strings.DisplayInfoTypes_Upload); break;
                        case DisplayInfoTypes.UploadPercent:
                            result.Add(CIV.strings.DisplayInfoTypes_UploadPercent); break;
                        case DisplayInfoTypes.Download:
                            result.Add(CIV.strings.DisplayInfoTypes_Download); break;
                        case DisplayInfoTypes.DownloadPercent:
                            result.Add(CIV.strings.DisplayInfoTypes_DownloadPercent); break;
                        case DisplayInfoTypes.Combined:
                            result.Add(CIV.strings.DisplayInfoTypes_Combined); break;
                        case DisplayInfoTypes.CombinedPercent:
                            result.Add(CIV.strings.DisplayInfoTypes_CombinedPercent); break;
                        case DisplayInfoTypes.CombinedOnTotal:
                            result.Add(CIV.strings.DisplayInfoTypes_CombinedOnTotal); break;
                        case DisplayInfoTypes.AverageCombined:
                            result.Add(CIV.strings.DisplayInfoTypes_AverageCombined); break;
                        case DisplayInfoTypes.SuggestCombined:
                            result.Add(CIV.strings.DisplayInfoTypes_SuggestCombined); break;
                        case DisplayInfoTypes.EstimateCombined:
                            result.Add(CIV.strings.DisplayInfoTypes_EstimateCombined); break;
                        case DisplayInfoTypes.EstimateTotalCombined:
                            result.Add(CIV.strings.DisplayInfoTypes_EstimateTotalCombined); break;
                        case DisplayInfoTypes.SuggestCombinedPercent:
                            result.Add(CIV.strings.DisplayInfoTypes_SuggestCombinedPercent); break;
                        case DisplayInfoTypes.TheoryDailyCombined:
                            result.Add(CIV.strings.DisplayInfoTypes_TheoryDailyCombined); break;
                        case DisplayInfoTypes.TheoryDailyCombinedPercent:
                            result.Add(CIV.strings.DisplayInfoTypes_TheoryDailyCombinedPercent); break;
                        case DisplayInfoTypes.TheoryCombinedDifference:
                            result.Add(CIV.strings.DisplayInfoTypes_TheoryCombinedDifference); break;
                        case DisplayInfoTypes.UploadDownloadGraph:
                            result.Add(CIV.strings.DisplayInfoTypes_UploadDownloadGraph); break;
                        case DisplayInfoTypes.CombinedGraph:
                            result.Add(CIV.strings.DisplayInfoTypes_CombinedGraph); break;
                        case DisplayInfoTypes.HistoryGraph:
                            result.Add(CIV.strings.DisplayInfoTypes_HistoryGraph); break;
                        default:
                            result.Add("?"); break;
                    }
                }
            }

            return result;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
