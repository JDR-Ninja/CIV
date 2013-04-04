using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class DateConverter : ConverterMarkupExtension<DateConverter>
    {
        public DateConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dateValue = (DateTime)value;

            if (dateValue == DateTime.MinValue)
                return null;
            else if (parameter != null)
            {
                if ((string)parameter == "date")
                    return dateValue.ToShortDateString();
                else
                    return dateValue.ToString((string)parameter);
            }
            else
                return dateValue.ToShortDateString();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
