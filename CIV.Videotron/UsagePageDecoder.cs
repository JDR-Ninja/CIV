using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using CIV.Common;
using System.IO;

namespace Videotron
{
    public class UsagePageDecoder
    {
        public List<Period> BillPeriods;
        public List<VideotronDailyUsage> DailyUsages;
        public List<string> DecodeErrors;

        public double Download;
        public double Upload;
        //public double TotalCombined;

        public DateTime PeriodStart;
        public DateTime PeriodEnd;

        public Exception decodeException;

        public UsagePageDecoder()
        {
            BillPeriods = new List<Period>();
            DailyUsages = new List<VideotronDailyUsage>();
            DecodeErrors = new List<string>();
            Download = 0;
            Upload = 0;
            //TotalCombined = 0;
        }

        public bool Extract(string source)
        {
            decodeException = null;
            BillPeriods.Clear();
            DailyUsages.Clear();
            DecodeErrors.Clear();
            Download = 0;
            Upload = 0;
            //TotalCombined = 0;
            int delta = 1;
            Double doubleValue;
            System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowCurrencySymbol;
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-CA");
            

            Match match;

            // An unexpected problem
            //<div[^>]+class=\"error\"[^>]*>(?<Msg>[^<]*(<br\\s*/?>)?[^<]*)</div>
            //match = Regex.Match(source, "<div[^>]+class=\"error\"[^>]*>(?<Msg>[^<]*(<br\\s*/?>)?[^<]*)</div>", RegexOptions.IgnoreCase);
            //if (match.Success)
            //    throw new DecodeFailedException(DecodeFailedTypes.Unexpected, source) { Message = match.Groups["Msg"].Value };
            //else
            //{
            //    match = Regex.Match(source, "<div[^>]+class=\"error\"[^>]*>(?<Msg>[^<]*<br\\s*/?>[^<]*)</div>", RegexOptions.IgnoreCase);
            //    if (match.Success)
            //        throw new DecodeFailedException(DecodeFailedTypes.Unexpected, source) { Message = match.Groups["Msg"].Value };
            //}

            // unité
            //match = Regex.Match(source, "<p class=\"arrow-down-yellow\">[a-z]*<br/><span class=\"txt-medium\">\\d+(\\.|,)?\\d*\\s*(&nbsp;)?\\s*(?<Unit>(g|m|k)o)", RegexOptions.IgnoreCase);
            match = Regex.Match(source, "<input[^>]+name=\"dataVolumeUnitCode\"[^>]*value=\"(?<Unit>G|M|K)(B|O)\"[^>]*>", RegexOptions.IgnoreCase);


            // Contre vérification, on ne sait jamais...
            if (!match.Success)
                match = Regex.Match(source, "<a[^>]*active[^>]*>(?<Unit>G|M|K)(O|B)</a>", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                if (match.Groups["Unit"].Value.ToUpper() == "G")
                    delta = 1048576;
                else if (match.Groups["Unit"].Value.ToUpper() == "M")
                    delta = 1024;
            }
            else
                throw new DecodeFailedException(DecodeFailedTypes.Unit, source);

            // Download
            //match = Regex.Match(source, "<p class=\"arrow-down-yellow\">[a-z]*<br/><span class=\"txt-medium\">(?<Download>\\d+(\\.|,)?\\d*)", RegexOptions.IgnoreCase);
            match = Regex.Match(source, @"download_label[^0-9]+(?<Download>[0-9,\.]+)", RegexOptions.IgnoreCase);
            if (match.Success && Double.TryParse(match.Groups["Download"].Value, style, culture, out doubleValue))
                Download = doubleValue * delta;
            else
                throw new DecodeFailedException(DecodeFailedTypes.Download, source);

            // Upload
            //match = Regex.Match(source, "<p class=\"arrow-up-blue\">[a-z]*<br/><span class=\"txt-medium\">(?<Upload>\\d+(\\.|,)?\\d*)", RegexOptions.IgnoreCase);
            match = Regex.Match(source, @"upload_label[^0-9]+(?<Upload>[0-9,\.]+)", RegexOptions.IgnoreCase);
            if (match.Success && Double.TryParse(match.Groups["Upload"].Value, style, culture, out doubleValue))
                Upload = doubleValue * delta;
            else
                throw new DecodeFailedException(DecodeFailedTypes.Upload, source);

            // TotalCombined
            //match = Regex.Match(source, "<p><span class=\"txt-emphasized\">[a-z]*</span><br/><span class=\"txt-medium\">(?<TotalCombined>\\d+(\\.|,)?\\d*)", RegexOptions.IgnoreCase);
            /*match = Regex.Match(source, @"total_label[^0-9]+(?<TotalCombined>[0-9,\.]+)", RegexOptions.IgnoreCase);
            if (match.Success && Double.TryParse(match.Groups["TotalCombined"].Value, style, culture, out doubleValue))
                TotalCombined = doubleValue * delta;
            else
                throw new DecodeFailedException(DecodeFailedTypes.TotalCombined, source);*/

            // Période affiché
            // <h3>Usage from  December 4, 2010 to January 3, 2011</h3>
            // <h3>Usagefrom December 4, 2010 to January 3, 2011</h3>
            // <h3>Votre consommation internetdu 4 décembre 2010 au 3 janvier 2011</h3>
            //match = Regex.Match(source, "<h3>Votre consommation internetdu (?<PeriodStartDay>[0-9]+) (?<PeriodStartMonth>[a-zéû]+) au (?<PeriodEndDay>[0-9]+) (?<PeriodEndMonth>[a-zéû]+) (?<PeriodEndYear>[0-9]{4})", RegexOptions.IgnoreCase);


            match = Regex.Match(source, @"<h3>Usage\s*from\s*(?<PeriodStartMonth>[a-zéû]+)\s*(?<PeriodStartDay>[0-9]+)[\s,]+[0-9\s]*to\s*(?<PeriodEndMonth>[a-zéû]+)\s*(?<PeriodEndDay>[0-9]+)[\s,]+(?<PeriodEndYear>[0-9]{4})", RegexOptions.IgnoreCase);
            //match = Regex.Match(source, @"<h3>Votre consommation internetdu (?<PeriodStartDay>[0-9]+) (?<PeriodStartMonth>[a-zéû]+) [0-9\s]*au (?<PeriodEndDay>[0-9]+) (?<PeriodEndMonth>[a-zéû]+) (?<PeriodEndYear>[0-9]{4})[^>]*", RegexOptions.IgnoreCase);

            if (!match.Success)
                match = Regex.Match(source, @"<h3>Consommation\s*du\s*(?<PeriodStartDay>[0-9]+)\s*(?<PeriodStartMonth>[a-zéû]+)\s*[0-9\s]*au\s*(?<PeriodEndDay>[0-9]+)\s*(?<PeriodEndMonth>[a-zéû]+)\s*(?<PeriodEndYear>[0-9]{4})[^>]*", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                PeriodEnd = DateTime.ParseExact(String.Format("{0}-{1}-{2}",
                                                                match.Groups["PeriodEndYear"].Value,
                                                                MonthStringToInt(match.Groups["PeriodEndMonth"].Value),
                                                                match.Groups["PeriodEndDay"].Value.PadLeft(2, '0')),
                                                "yyyy-MM-dd", CultureInfo.InvariantCulture);
                int year = PeriodEnd.Year;
                if (PeriodEnd.Month == 1 && MonthStringToInt(match.Groups["PeriodStartMonth"].Value) == "12")
                    year -= 1;

                PeriodStart = DateTime.ParseExact(String.Format("{0}-{1}-{2}",
                                                                year,
                                                                MonthStringToInt(match.Groups["PeriodStartMonth"].Value),
                                                                match.Groups["PeriodStartDay"].Value.PadLeft(2, '0')),
                                                    "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else
                throw new DecodeFailedException(DecodeFailedTypes.ShowPeriods, source);

            // Périodes disponibles
            match = Regex.Match(source, "<select[^>]*>(?<options><option[^>]*>([^<]+)</option>)+</select>", RegexOptions.IgnoreCase);
                
            if (match.Success)
            {
                Match matchPeriods;
                int year;
                DateTime start;
                DateTime end;

                for (int i = 0; i < match.Groups[1].Captures.Count; i++)
                {
                    // En cours (du 4 juillet au 3 août 2010)
                    // En cours (du 4 décembre 2010 au 3 janvier 2011)
                        
                    //BillPeriods.Add(match.Groups[1].Captures[i].Value);
                    //matchPeriods = Regex.Match(match.Groups[1].Captures[i].Value, @"(?<StartDay>\d+)\s+(?<StartMonth>janvier|février|mars|avril|mai|juin|juillet|août|septembre|octobre|novembre|décembre)[^\d]+(?<EndDay>\d+)\s+(?<EndMonth>janvier|février|mars|avril|mai|juin|juillet|août|septembre|octobre|novembre|décembre)\s+(?<EndYear>\d{4})", RegexOptions.IgnoreCase);
                    //matchPeriods = Regex.Match(match.Groups[1].Captures[i].Value, @"(?<StartDay>\d+)\s+(?<StartMonth>janvier|février|mars|avril|mai|juin|juillet|août|septembre|octobre|novembre|décembre) [0-9\s]*[^\d]+(?<EndDay>\d+)\s+(?<EndMonth>janvier|février|mars|avril|mai|juin|juillet|août|septembre|octobre|novembre|décembre)\s+(?<EndYear>\d{4})", RegexOptions.IgnoreCase);
                    matchPeriods = Regex.Match(match.Groups[1].Captures[i].Value, @"(?<StartMonth>[a-zéû]+)\s+(?<StartDay>\d+)[\s,]+[0-9\s]*to\s+(?<EndMonth>[a-zéû]+)\s+(?<EndDay>\d+)[\s,]+(?<EndYear>\d{4})", RegexOptions.IgnoreCase);

                    if (!matchPeriods.Success)
                        matchPeriods = Regex.Match(match.Groups[1].Captures[i].Value, @"(?<StartDay>\d+)\s*(?<StartMonth>[a-zéû]+)\s*[0-9\s]*[^\d]+(?<EndDay>\d+)\s*(?<EndMonth>[a-zéû]+)\s*(?<EndYear>\d{4})", RegexOptions.IgnoreCase);

                    if (matchPeriods.Success)
                    {
                        try
                        {
                            end = DateTime.ParseExact(String.Format("{0}-{1}-{2}",
                                                        matchPeriods.Groups["EndYear"].Value,
                                                        MonthStringToInt(matchPeriods.Groups["EndMonth"].Value),
                                                        matchPeriods.Groups["EndDay"].Value.PadLeft(2, '0')),
                                                        "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            year = end.Year;

                            if (end.Month == 1 && MonthStringToInt(matchPeriods.Groups["EndMonth"].Value) == "12")
                                year -= 1;

                            start = DateTime.ParseExact(String.Format("{0}-{1}-{2}",
                                                        year,
                                                        MonthStringToInt(matchPeriods.Groups["StartMonth"].Value),
                                                        matchPeriods.Groups["StartDay"].Value.PadLeft(2, '0')),
                                                        "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            BillPeriods.Add(new Period() { Start = start, End = end });
                        }
                        catch
                        {
                            BillPeriods.Add(new Period() { Start = DateTime.MinValue, End = DateTime.MinValue.AddDays(1) });
                        }
                    }

                }
            }
            else
                throw new DecodeFailedException(DecodeFailedTypes.BillPeriods, source);

            // Historique
            match = Regex.Match(source, @"<td[^>]*>(([a-zéû]+ [0-9]{1,2})&nbsp;<br ?/?>)+</td><td[^>]*>(([\d,\.]+)<br ?/?>)+</td><td[^>]*>(([\d,\.]+)<br ?/?>)+</td><td[^>]*><table><?t?b?o?d?y?>?<tr><td[^>]*>(([\d,\.]+)<br ?/?>)+", RegexOptions.IgnoreCase);

            if (!match.Success)
                match = Regex.Match(source, @"<td[^>]*>(([0-9]{1,2} [a-zéû]+)&nbsp;<br ?/?>)+</td><td[^>]*>(([0-9]+.{1}[0-9]{2})<br ?/?>)+</td><td[^>]*>(([0-9]+.{1}[0-9]{2})<br ?/?>)+</td><td[^>]*><table><?t?b?o?d?y?>?<tr><td[^>]*>(([0-9]+.{1}[0-9]{2})<br ?/?>)+", RegexOptions.IgnoreCase);
        
            if (match.Success)
            {
                VideotronDailyUsage usage;

                DateTime daily;
                for (int i = 0; i < match.Groups[2].Captures.Count; i++)
                {
                    daily = DateStrToDateTime(match.Groups[2].Captures[i].Value);

                    usage = new VideotronDailyUsage();
                    usage.Day = daily;
                    usage.Month = daily.Year.ToString() + daily.Month.ToString().PadLeft(2, '0');
                    if (Double.TryParse(match.Groups[4].Captures[i].Value, style, culture, out doubleValue))
                        usage.Download = doubleValue;
                    if (Double.TryParse(match.Groups[6].Captures[i].Value, style, culture, out doubleValue))
                        usage.Upload = doubleValue;
                    if (Double.TryParse(match.Groups[8].Captures[i].Value, style, culture, out doubleValue))
                        usage.Total = doubleValue;

                    usage.Download = usage.Download * delta;
                    usage.Upload = usage.Upload * delta;
                    usage.Total = usage.Total * delta;
                    usage.Period = new Period() { Start = PeriodStart, End = PeriodEnd };

                    DailyUsages.Add(usage);
                }
            }
            else
                throw new DecodeFailedException(DecodeFailedTypes.History, source);

            return true;
        }

        private DateTime DateStrToDateTime(string text)
        {
            string day;
            string month;
            int year;

            Match dateMatch;

            dateMatch = Regex.Match(text, @"(?<month>[a-zéû]+)\s+(?<day>\d{1,2})", RegexOptions.IgnoreCase);

            if (!dateMatch.Success)
                dateMatch = Regex.Match(text, @"(?<day>\d{1,2})\s+(?<month>[a-zéû]+)", RegexOptions.IgnoreCase);


            if (dateMatch.Success)
            {
                day = dateMatch.Groups["day"].Value.PadLeft(2, '0');
                month = MonthStringToInt(dateMatch.Groups["month"].Value);

                if (month == PeriodEnd.Month.ToString().PadLeft(2, '0'))
                    year = PeriodEnd.Year;
                else
                    year = PeriodStart.Year;

                CultureInfo provider = CultureInfo.InvariantCulture;
                return DateTime.ParseExact(String.Format("{0}-{1}-{2}", year, month, day), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            return DateTime.Now;
        }

        private string MonthStringToInt(string month)
        {
            if (Regex.Match(month, "janvier|january", RegexOptions.IgnoreCase).Success)
                return "01";
            else if (Regex.Match(month, "f(e|é)vrier|February", RegexOptions.IgnoreCase).Success)
                return "02";
            else if (Regex.Match(month, "mars|March", RegexOptions.IgnoreCase).Success)
                return "03";
            else if (Regex.Match(month, "avril|april", RegexOptions.IgnoreCase).Success)
                return "04";
            else if (Regex.Match(month, "mai|may", RegexOptions.IgnoreCase).Success)
                return "05";
            else if (Regex.Match(month, "juin|june", RegexOptions.IgnoreCase).Success)
                return "06";
            else if (Regex.Match(month, "juillet|july", RegexOptions.IgnoreCase).Success)
                return "07";
            else if (Regex.Match(month, "ao(u|û)t|August", RegexOptions.IgnoreCase).Success)
                return "08";
            else if (Regex.Match(month, "septembre|September", RegexOptions.IgnoreCase).Success)
                return "09";
            else if (Regex.Match(month, "octobre|october", RegexOptions.IgnoreCase).Success)
                return "10";
            else if (Regex.Match(month, "novembre|november", RegexOptions.IgnoreCase).Success)
                return "11";
            else
                return "12";
        }
    }
}
