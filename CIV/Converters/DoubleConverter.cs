using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class DoubleConverter : ConverterMarkupExtension<DoubleConverter>
    {
        public DoubleConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double doubleValue = (double)value;

            if (parameter != null)
                return doubleValue.ToString((string)parameter);
            else
                return doubleValue.ToString();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
