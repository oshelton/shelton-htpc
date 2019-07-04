using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.Dtos.Layout
{
    /// <summary>
    /// The automatic background rotation scheme to be used by Shelton HTPC.
    /// </summary>
    public enum BackgroundRotationScheme
    {
        ///Background sets are specified manually in the configuration application.
        NONE,
        ///A new background set will be chosen on Shelton HTPC startup.
        STARTUP,
        ///A new background set will be chosen on Shelton HTPC startup and every hour therafter.
        HOURLY
    }
}
