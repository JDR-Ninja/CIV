using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV
{
    public class GreaterThanConverter : ConverterMarkupExtension<GreaterThanConverter>
    {
        public GreaterThanConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value) > System.Convert.ToInt32(parameter);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
