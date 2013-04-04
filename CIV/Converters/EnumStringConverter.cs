using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CIV
{
    public class EnumStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            //return value.ToString();
            //return value;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return (UnitTypes)Enum.Parse(typeof(UnitTypes), value.ToString(), true);
            //KeyValuePair<Enum, String> selectedValue = value as KeyValuePair<Enum, String>;

            //KeyValuePair<Enum, String> selectedValue = (KeyValuePair<Enum, String>)value;

            //return selectedValue.Key;
            return value;
        }
    }
}
