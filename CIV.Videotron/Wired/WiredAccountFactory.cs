using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Videotron.Api.Xml;
using CIV.Common;

namespace Videotron.Wired
{
    public class WiredAccountFactory
    {
        public static WiredAccount CreateWiredAccount(WiredInternetUsage wiredInternetUsage, string username)
        {
            WiredAccount result = new WiredAccount();

            result.Username = username;
            result.PeriodStart = wiredInternetUsage.PeriodStartDate;
            result.PeriodEnd = wiredInternetUsage.PeriodEndDate;
            result.DaysElapsed = wiredInternetUsage.DaysFromStart;
            result.DaysRemaining = wiredInternetUsage.DaysToEnd;

            foreach (Message message in wiredInternetUsage.Messages.Message)
                result.Messages.Add(WiredMessageFactory.CreateMessage(message.Code,
                                                                      message.Severity,
                                                                      message.Text));

            WiredInternetAccountUsage accountUsage = wiredInternetUsage.InternetAccounts.WiredInternetAccountUsage.FirstOrDefault(x => x.InternetAccountNo == username);

            if (accountUsage != null)
            {
                // Les données arrivent en octets, on sauvegarde en kilo-octets
                result.MaxDownloadBytes = accountUsage.MaxDownloadBytes / 1024;
                result.MaxUploadBytes = accountUsage.MaxUploadBytes / 1024;
                result.MaxCombinedBytes = accountUsage.MaxCombinedBytes / 1024;
                result.DownloadedBytes = accountUsage.DownloadedBytes / 1024;
                result.UploadedBytes = accountUsage.UploadedBytes / 1024;
                result.DownloadedPercent = accountUsage.DownloadedPercent;
                result.UploadedPercent = accountUsage.UploadedPercent;
                result.CombinedPercent = accountUsage.CombinedPercent;

                foreach (WiredInternetDailyUsage usage in accountUsage.DailyUsage.WiredInternetDailyUsage)
                {
                    result.DailyUsage.Add(new WiredDailyUsage(usage.Date,
                                                              usage.UploadedBytes / 1024,
                                                              usage.DownloadedBytes / 1024)
                                                              { Period = new Period(result.PeriodStart, result.PeriodEnd) });
                }

                foreach (Message message in accountUsage.Messages.Message)
                    result.Messages.Add(WiredMessageFactory.CreateMessage(message.Code,
                                                                          message.Severity,
                                                                          message.Text));
            }

            return result;
        }
    }
}
