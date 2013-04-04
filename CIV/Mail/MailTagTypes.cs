using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Mail
{
    /// <summary>
    /// Liste des tags gérés par le moteur
    /// </summary>
    public enum MailTagTypes
    {
        None,
        Name,
        Username,
        PeriodStart,
        PeriodEnd,
        DayRemaining,
        UploadPercent,
        Upload,
        DownloadPercent,
        Download,
        TotalCombinedPercent,
        TotalCombinedRemaining,
        TotalCombinedMax,
        TotalCombined,
        SuggestDailyUsage,
        Overcharge,
        Now,
        LastUpdate,
        AverageCombined,
        SuggestCombined,
        EstimateCombined,
        SuggestCombinedPercent,
        TheoryDailyCombined,
        TheoryDailyCombinedPercent,
        TheoryCombined,
        TheoryCombinedDifference
    }
}
