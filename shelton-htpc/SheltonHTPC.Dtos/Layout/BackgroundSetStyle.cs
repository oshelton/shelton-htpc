using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.Dtos.Layout
{
    /// <summary>
    /// Style of background used by this background set.
    /// </summary>
    public enum BackgroundSetStyle
    {
        /// <summary>
        /// There will be a background image per scene.
        /// </summary>
        PER_SCENE,
        /// <summary>
        /// All scenes will use a single image that will be panned.  Works best with panoramic images.
        /// </summary>
        SPANNED
    }
}
