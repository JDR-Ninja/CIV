using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron.Wired
{
    internal class WiredMessageFactory
    {
        public static WiredMessage CreateMessage(string code, string severity, string text)
        {
            WiredMessage result = new WiredMessage();

            switch (code)
            {
                case "server_error": result.Code = WiredMessageCodeTypes.ServerError; break;
                case "blocked_ip": result.Code = WiredMessageCodeTypes.BlockedIp; break;
                case "invalidToken": result.Code = WiredMessageCodeTypes.InvalidToken; break;
                case "invalidTokenClass": result.Code = WiredMessageCodeTypes.InvalidTokenClass; break;
                case "noProfile": result.Code = WiredMessageCodeTypes.NoProfile; break;
                case "noProfile.23006": result.Code = WiredMessageCodeTypes.NoProfileTemporary; break;
                case "noUsage.23009": result.Code = WiredMessageCodeTypes.NoUsageTemporary; break;
                case "noUsage.corrupted": result.Code = WiredMessageCodeTypes.NoUsageCorrupted; break;
                case "detailed_usage": result.Code = WiredMessageCodeTypes.DetailedUsage; break;
                case "notification": result.Code = WiredMessageCodeTypes.Notification; break;
            }

            switch (severity)
            {
                case "error": result.Severity = WiredMessageSeverityTypes.Error; break;
                case "info": result.Severity = WiredMessageSeverityTypes.Info; break;
                case "warning": result.Severity = WiredMessageSeverityTypes.Warning; break;
            }

            result.Text = text;

            return result;
        }
    }
}
