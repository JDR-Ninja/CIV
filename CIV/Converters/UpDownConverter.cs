using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class UpDownConverter : ConverterMarkupExtension<UpDownConverter>
    {
        public UpDownConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double doubleValue = (double)value;
            string token;

            if (doubleValue > 0)
               token = CIV.strings.ClientDashboard_Under;
            else
                token = CIV.strings.ClientDashboard_Over;

            return String.Format("{0} {1}",
                                 CIV.Common.UnitsConverter.SIUnitToString(Math.Abs(doubleValue),
                                                                          ProgramSettings.Instance.ShowUnitType),
                                 token);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
