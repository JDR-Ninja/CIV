using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videotron.Wired
{
    public enum WiredMessageCodeTypes { ServerError,
                                        BlockedIp,
                                        InvalidToken,
                                        InvalidTokenClass,
                                        NoProfile,
                                        NoProfileTemporary,
                                        NoUsageTemporary,
                                        NoUsageCorrupted,
                                        DetailedUsage,
                                        Notification}
}
