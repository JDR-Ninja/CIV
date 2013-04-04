using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class TimeSpanConverter : ConverterMarkupExtension<TimeSpanConverter>
    {
        public TimeSpanConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan timeSpanValue = (TimeSpan)value;

            StringBuilder display = new StringBuilder();

            // La nouvelle période n'est pas encore arrivé
            if (timeSpanValue.TotalMinutes > 0)
            {
                // Jours
                if (timeSpanValue.Days > 0)
                    display.Append(String.Format("{0} {1} ", timeSpanValue.Days, timeSpanValue.Days == 1 ? CIV.strings.ClientDashboard_Day : CIV.strings.ClientDashboard_Days));

                // Heures
                if (timeSpanValue.Hours > 0)
                    display.Append(String.Format("{0} {1} ", timeSpanValue.Hours, timeSpanValue.Hours == 1 ? CIV.strings.ClientDashboard_Hour : CIV.strings.ClientDashboard_Hours));

                // Minutes
                if (timeSpanValue.Minutes > 0)
                    display.Append(String.Format("{0} {1}", timeSpanValue.Minutes, timeSpanValue.Minutes == 1 ? CIV.strings.ClientDashboard_Minute : CIV.strings.ClientDashboard_Minutes));

                return display.ToString().TrimEnd();
            }
            else
                return "0";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
