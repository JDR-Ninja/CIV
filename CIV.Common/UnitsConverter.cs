using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class UnitsConverter
    {
        public static double ConvertKo(double value, SIUnitTypes convert)
        {
            switch (convert)
            {
                case SIUnitTypes.Mo: return value / 1024;
                case SIUnitTypes.Go: return value / 1048576;
                default: return value;
            }
        }

        public static string SIUnitToString(double value, SIUnitTypes convert)
        {
            switch (convert)
            {
                case SIUnitTypes.Mo: return String.Format("{0:N2} Mo", (double)value / 1024);
                case SIUnitTypes.Go: return String.Format("{0:N2} Go", (double)value / 1048576);
                default: return String.Format("{0:N2} ko", (double)value);
            }
        }

        public static string UnitToString(double value, UnitTypes convert)
        {
            switch (convert)
            {
                case UnitTypes.Mio: return String.Format("{0:N2} Mio", (double)value / 1000);
                case UnitTypes.Gio: return String.Format("{0:N2} Gio", (double)value / 1000000);
                default: return String.Format("{0:N2} kio", (double)value);
            }
        }
    }
}
