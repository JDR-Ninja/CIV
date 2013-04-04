using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace CIV
{
    public class IntegerConverter : ConverterMarkupExtension<IntegerConverter>
    {
        public IntegerConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return value.ToString();
            else
                return "0";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int intValue;

            if (Int32.TryParse((string)value, out intValue))
                return intValue;
            else
            {
                string strValue = Regex.Replace((string)value, @"[^\d]", String.Empty);
                if (String.IsNullOrEmpty(strValue))
                    return 0;
                else
                    return strValue;
            }
        }
    }
}
