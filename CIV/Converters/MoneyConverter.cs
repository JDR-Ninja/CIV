using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class MoneyConverter : ConverterMarkupExtension<MoneyConverter>
    {
        public MoneyConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double doubleValue = (double)value;

            return doubleValue.ToString("C");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
