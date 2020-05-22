using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.FrontEndData.Entites
{
    /// <summary>
    /// Base view model for layout settings.
    /// </summary>
    public sealed class LayoutSettings 
    {  
        /// <summary>
        /// Construct an instance of this class from a corresponding dto object.
        /// </summary>
        public LayoutSettings(LayoutSettingsDto settingsDto)
        {
            BackgroundRotationScheme = settingsDto.BackgroundRotationScheme;
        }
        
        /// <summary>
        /// Currently used background rotation scheme.
        /// </summary>
        public Dtos.Layout.BackgroundRotationScheme BackgroundRotationScheme { get; private set; }
    }
}
