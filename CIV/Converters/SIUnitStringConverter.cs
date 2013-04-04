using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIV.Common;

namespace CIV
{
    public class SIUnitStringConverter : ConverterMarkupExtension<SIUnitStringConverter>
    {
        public SIUnitStringConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return CIV.Common.UnitsConverter.SIUnitToString((double)value, parameter == null ? ProgramSettings.Instance.ShowUnitType : (SIUnitTypes)parameter);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
