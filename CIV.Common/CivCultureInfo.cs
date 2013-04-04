using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CIV.Common
{
    public class CivCultureInfo
    {
        public string DisplayName;

        public string DateSeparator;
        public string FirstDayOfWeek;
        public string FullDateTimePattern;
        public string LongDatePattern;
        public string LongTimePattern;
        public string MonthDayPattern;
        public string ShortDatePattern;
        public string ShortTimePattern;
        public string SortableDateTimePattern;
        public string TimeSeparator;
        public string UniversalSortableDateTimePattern;
        public string YearMonthPattern;

        public int CurrencyDecimalDigits;
        public string CurrencyDecimalSeparator;
        public string CurrencyGroupSeparator;
        public int CurrencyNegativePattern;
        public int CurrencyPositivePattern;
        public string CurrencySymbol;
        public string NaNSymbol;
        public string NegativeInfinitySymbol;
        public string NegativeSign;
        public int NumberDecimalDigits;
        public string NumberDecimalSeparator;
        public string NumberGroupSeparator;
        public int NumberNegativePattern;
        public int PercentDecimalDigits;
        public string PercentDecimalSeparator;
        public string PercentGroupSeparator;
        public int PercentNegativePattern;
        public int PercentPositivePattern;
        public string PercentSymbol;
        public string PerMilleSymbol;
        public string PositiveInfinitySymbol;
        public string PositiveSign;

        public CivCultureInfo()
        {

        }

        public CivCultureInfo(CultureInfo cultureInfo)
        {
            DisplayName = cultureInfo.DisplayName;

            DateSeparator= cultureInfo.DateTimeFormat.DateSeparator;
            FirstDayOfWeek= cultureInfo.DateTimeFormat.FirstDayOfWeek.ToString();
            FullDateTimePattern= cultureInfo.DateTimeFormat.FullDateTimePattern;
            LongDatePattern= cultureInfo.DateTimeFormat.LongDatePattern;
            LongTimePattern= cultureInfo.DateTimeFormat.LongTimePattern;
            MonthDayPattern= cultureInfo.DateTimeFormat.MonthDayPattern;
            ShortDatePattern= cultureInfo.DateTimeFormat.ShortDatePattern;
            ShortTimePattern= cultureInfo.DateTimeFormat.ShortTimePattern;
            SortableDateTimePattern= cultureInfo.DateTimeFormat.SortableDateTimePattern;
            TimeSeparator= cultureInfo.DateTimeFormat.TimeSeparator;
            UniversalSortableDateTimePattern= cultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
            YearMonthPattern= cultureInfo.DateTimeFormat.YearMonthPattern;

            CurrencyDecimalDigits = cultureInfo.NumberFormat.CurrencyDecimalDigits;
            CurrencyDecimalSeparator = cultureInfo.NumberFormat.CurrencyDecimalSeparator;
            CurrencyGroupSeparator = cultureInfo.NumberFormat.CurrencyGroupSeparator;
            CurrencyNegativePattern = cultureInfo.NumberFormat.CurrencyNegativePattern;
            CurrencyPositivePattern = cultureInfo.NumberFormat.CurrencyPositivePattern;
            CurrencySymbol = cultureInfo.NumberFormat.CurrencySymbol;
            NaNSymbol = cultureInfo.NumberFormat.NaNSymbol;
            NegativeInfinitySymbol = cultureInfo.NumberFormat.NegativeInfinitySymbol;
            NegativeSign = cultureInfo.NumberFormat.NegativeSign;
            NumberDecimalDigits = cultureInfo.NumberFormat.NumberDecimalDigits;
            NumberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
            NumberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator;
            NumberNegativePattern = cultureInfo.NumberFormat.NumberNegativePattern;
            PercentDecimalDigits = cultureInfo.NumberFormat.PercentDecimalDigits;
            PercentDecimalSeparator = cultureInfo.NumberFormat.PercentDecimalSeparator;
            PercentGroupSeparator = cultureInfo.NumberFormat.PercentGroupSeparator;
            PercentNegativePattern = cultureInfo.NumberFormat.PercentNegativePattern;
            PercentPositivePattern = cultureInfo.NumberFormat.PercentPositivePattern;
            PercentSymbol = cultureInfo.NumberFormat.PercentSymbol;
            PerMilleSymbol = cultureInfo.NumberFormat.PerMilleSymbol;
            PositiveInfinitySymbol = cultureInfo.NumberFormat.PositiveInfinitySymbol;
            PositiveSign = cultureInfo.NumberFormat.PositiveSign;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine(String.Format("DateSeparator :\t{0}", DateSeparator));
            result.AppendLine(String.Format("FirstDayOfWeek :\t{0}", FirstDayOfWeek));
            result.AppendLine(String.Format("FullDateTimePattern :\t{0}", FullDateTimePattern));
            result.AppendLine(String.Format("LongDatePattern :\t{0}", LongDatePattern));
            result.AppendLine(String.Format("LongTimePattern :\t{0}", LongTimePattern));
            result.AppendLine(String.Format("MonthDayPattern :\t{0}", MonthDayPattern));
            result.AppendLine(String.Format("ShortDatePattern :\t{0}", ShortDatePattern));
            result.AppendLine(String.Format("ShortTimePattern :\t{0}", ShortTimePattern));
            result.AppendLine(String.Format("SortableDateTimePattern :\t{0}", SortableDateTimePattern));
            result.AppendLine(String.Format("TimeSeparator :\t{0}", TimeSeparator));
            result.AppendLine(String.Format("UniversalSortableDateTimePattern :\t{0}", UniversalSortableDateTimePattern));
            result.AppendLine(String.Format("YearMonthPattern :\t{0}", YearMonthPattern));
            result.AppendLine(String.Format("CurrencyDecimalDigits :\t{0}", CurrencyDecimalDigits));
            result.AppendLine(String.Format("CurrencyDecimalSeparator :\t{0}", CurrencyDecimalSeparator));
            result.AppendLine(String.Format("CurrencyGroupSeparator :\t{0}", CurrencyGroupSeparator));
            result.AppendLine(String.Format("CurrencyNegativePattern :\t{0}", CurrencyNegativePattern));
            result.AppendLine(String.Format("CurrencyPositivePattern :\t{0}", CurrencyPositivePattern));
            result.AppendLine(String.Format("CurrencySymbol :\t{0}", CurrencySymbol));
            result.AppendLine(String.Format("NaNSymbol :\t{0}", NaNSymbol));
            result.AppendLine(String.Format("NegativeInfinitySymbol :\t{0}", NegativeInfinitySymbol));
            result.AppendLine(String.Format("NegativeSign :\t{0}", NegativeSign));
            result.AppendLine(String.Format("NumberDecimalDigits :\t{0}", NumberDecimalDigits));
            result.AppendLine(String.Format("NumberDecimalSeparator :\t{0}", NumberDecimalSeparator));
            result.AppendLine(String.Format("NumberGroupSeparator :\t{0}", NumberGroupSeparator));
            result.AppendLine(String.Format("NumberNegativePattern :\t{0}", NumberNegativePattern));
            result.AppendLine(String.Format("PercentDecimalDigits :\t{0}", PercentDecimalDigits));
            result.AppendLine(String.Format("PercentDecimalSeparator :\t{0}", PercentDecimalSeparator));
            result.AppendLine(String.Format("PercentGroupSeparator :\t{0}", PercentGroupSeparator));
            result.AppendLine(String.Format("PercentNegativePattern :\t{0}", PercentNegativePattern));
            result.AppendLine(String.Format("PercentPositivePattern :\t{0}", PercentPositivePattern));
            result.AppendLine(String.Format("PercentSymbol :\t{0}", PercentSymbol));
            result.AppendLine(String.Format("PerMilleSymbol :\t{0}", PerMilleSymbol));
            result.AppendLine(String.Format("PositiveInfinitySymbol :\t{0}", PositiveInfinitySymbol));
            result.AppendLine(String.Format("PositiveSign :\t{0}", PositiveSign));

            return result.ToString();
        }
    }
}
