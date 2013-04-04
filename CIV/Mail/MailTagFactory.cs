using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Videotron;

namespace CIV.Mail
{
    /// <summary>
    /// Classe qui fait la correspondance entre les tags en string et les objets correspondant
    /// </summary>
    public class MailTagFactory
    {
        private VideotronAccount _account;

        public MailTagFactory(VideotronAccount account)
        {
            _account = account;
        }

        /// <summary>
        /// Cherche la correspondance d'un tag en string
        /// </summary>
        /// <param name="name">Le tag en string</param>
        /// <returns>Le type de tag</returns>
        public MailTagTypes GetType(string name)
        {
            switch (name)
            {
                case "NAME" : return MailTagTypes.Name;
                case "USERNAME" : return MailTagTypes.Username;
                case "PERIOD_START" : return MailTagTypes.PeriodStart;
                case "PERIOD_END" : return MailTagTypes.PeriodEnd;
                case "DAY_REMAINING" : return  MailTagTypes.DayRemaining;
                case "UPLOAD_PERCENT" : return MailTagTypes.UploadPercent;
                case "UPLOAD" : return MailTagTypes.Upload;
                case "DOWNLOAD_PERCENT" : return MailTagTypes.DownloadPercent;
                case "DOWNLOAD" : return MailTagTypes.Download;
                case "TOTAL_COMBINED_PERCENT" : return MailTagTypes.TotalCombinedPercent;
                case "TOTAL_COMBINED_REMAINING" : return MailTagTypes.TotalCombinedRemaining;
                case "TOTAL_COMBINED_MAX" : return MailTagTypes.TotalCombinedMax;
                case "TOTAL_COMBINED" : return MailTagTypes.TotalCombined;
                case "SUGGEST_DAILY_USAGE" : return MailTagTypes.SuggestDailyUsage;
                case "OVERCHARGE" : return MailTagTypes.Overcharge;
                case "NOW" : return MailTagTypes.Now;
                case "LAST_UPDATE" : return MailTagTypes.LastUpdate;
                case "AVERAGE_COMBINED" : return MailTagTypes.AverageCombined;
                case "SUGGEST_COMBINED" : return MailTagTypes.SuggestCombined;
                case "ESTIMATE_COMBINED" : return MailTagTypes.EstimateCombined;
                case "SUGGEST_COMBINED_PERCENT" : return MailTagTypes.SuggestCombinedPercent;
                case "THEORY_DAILY_COMBINED" : return MailTagTypes.TheoryDailyCombined;
                case "THEORY_DAILY_COMBINED_PERCENT": return MailTagTypes.TheoryDailyCombinedPercent;
                case "THEORY_COMBINED" : return MailTagTypes.TheoryCombined;
                case "THEORY_COMBINED_DIFFERENCE" : return MailTagTypes.TheoryCombinedDifference;
                default: return MailTagTypes.None;
            }
        }

        public bool IsDateTime(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.PeriodStart: return true;
                case MailTagTypes.PeriodEnd: return true;
                case MailTagTypes.Now: return true;
                case MailTagTypes.LastUpdate: return true;
                default: return false;
            }
        }

        public DateTime GetDateTimeValue(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.PeriodStart: return _account.PeriodStart;
                case MailTagTypes.PeriodEnd: return _account.PeriodEnd;
                case MailTagTypes.Now: return DateTime.Now;
                case MailTagTypes.LastUpdate: return _account.LastUpdate;
                default: return DateTime.MinValue;
            }
        }

        public bool IsDouble(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.Upload: return true;
                case MailTagTypes.Download: return true;
                case MailTagTypes.TotalCombinedRemaining: return true;
                case MailTagTypes.TotalCombinedMax: return true;
                case MailTagTypes.TotalCombined: return true;
                case MailTagTypes.SuggestDailyUsage: return true;
                case MailTagTypes.AverageCombined: return true;
                case MailTagTypes.SuggestCombined: return true;
                case MailTagTypes.EstimateCombined: return true;
                case MailTagTypes.TheoryDailyCombined: return true;
                case MailTagTypes.TheoryCombined: return true;
                case MailTagTypes.TheoryCombinedDifference: return true;
                default: return false;
            }
        }

        public double GetDoubleValue(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.Upload: return _account.Upload;
                case MailTagTypes.Download: return _account.Download;
                case MailTagTypes.TotalCombinedRemaining: return _account.CombinedRemaining;
                case MailTagTypes.TotalCombinedMax: return _account.CombinedMaximum;
                case MailTagTypes.TotalCombined: return _account.Combined;
                case MailTagTypes.SuggestDailyUsage: return _account.SuggestCombined;
                case MailTagTypes.AverageCombined: return _account.AverageCombined;
                case MailTagTypes.SuggestCombined: return _account.SuggestCombined;
                case MailTagTypes.EstimateCombined: return _account.EstimateCombined;
                case MailTagTypes.TheoryDailyCombined: return _account.TheoryDailyCombined;
                case MailTagTypes.TheoryCombined: return _account.TheoryCombined;
                case MailTagTypes.TheoryCombinedDifference: return _account.TheoryCombinedVersusCombined;
                default: return 0.0;
            }
        }

        public bool IsInt(MailTagTypes type)
        {
            return type == MailTagTypes.DayRemaining;
        }

        public int GetIntValue(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.DayRemaining: return _account.DayRemaining.Days;
                default: return 0;
            }
        }

        public bool IsPercent(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.UploadPercent: return true;
                case MailTagTypes.DownloadPercent: return true;
                case MailTagTypes.TotalCombinedPercent: return true;
                case MailTagTypes.SuggestCombinedPercent: return true;
                case MailTagTypes.TheoryDailyCombinedPercent: return true;
                default: return false;
            }
        }

        public double GetPercentValue(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.UploadPercent: return _account.UploadPercent;
                case MailTagTypes.DownloadPercent: return _account.DownloadPercent;
                case MailTagTypes.TotalCombinedPercent: return _account.CombinedPercent;
                case MailTagTypes.SuggestCombinedPercent: return _account.SuggestCombinedPercent;
                case MailTagTypes.TheoryDailyCombinedPercent: return _account.TheoryDailyCombinedPercent;
                default: return 0.0;
            }
        }

        public bool IsCurrency(MailTagTypes type)
        {
            return type == MailTagTypes.Overcharge;
        }

        public double GetCurrencyValue(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.Overcharge: return _account.Overcharge;
                default: return 0.0;
            }
        }

        public string GetStringValue(MailTagTypes type)
        {
            switch (type)
            {
                case MailTagTypes.Name: return _account.DisplayName;
                case MailTagTypes.Username: return _account.Username;
                default: return String.Empty;
            }
        }
    }
}
