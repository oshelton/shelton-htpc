using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAspects.Core;

namespace SheltonHTPC.Data.Entities
{
    /// <summary>
    /// Base view model for layout settings.
    /// </summary>
    public class LayoutSettings : Model 
    {
        /// <summary>
        /// Collection of background rotation schemes.
        /// </summary>
        public static IEnumerable<string> BackgroundSchemas = new[]
        {
            ManualSchemeName,
            StartupSchemeName,
            HourlySchemeName
        };


        /// <summary>
        /// Default constructor so subclasses can create instances for duplication purposes.
        /// </summary>
        protected LayoutSettings() { }

        /// <summary>
        /// Construct an instance of this class from a corresponding dto object.
        /// </summary>
        public LayoutSettings(LayoutSettingsDto settingsDto)
        {
            switch (settingsDto.BackgroundRotationScheme)
            {
                case Dtos.Layout.BackgroundRotationScheme.NONE:
                    BackgroundRotationScheme = ManualSchemeName;
                    break;
                case Dtos.Layout.BackgroundRotationScheme.STARTUP:
                    BackgroundRotationScheme = StartupSchemeName;
                    break;
                case Dtos.Layout.BackgroundRotationScheme.HOURLY:
                    BackgroundRotationScheme = HourlySchemeName;
                    break;
            }
        }

        /// <summary>
        /// Id of the settings object.
        /// </summary>
        private Guid Id { get; set; }

        protected string _BackgroundRotationScheme = null;
        /// <summary>
        /// Currently used background rotation scheme.
        /// </summary>
        public string BackgroundRotationScheme
        {
            get => _BackgroundRotationScheme;
            set => SetPropertyBackingValue(value, ref _BackgroundRotationScheme);
        }

        /// <summary>
        /// Construct a dto object from this view model.
        /// </summary>
        public LayoutSettingsDto CreateDto()
        {
            Dtos.Layout.BackgroundRotationScheme schemeToUse = Dtos.Layout.BackgroundRotationScheme.NONE;
            switch (BackgroundRotationScheme)
            {
                case ManualSchemeName:
                    schemeToUse = Dtos.Layout.BackgroundRotationScheme.NONE;
                    break;
                case StartupSchemeName:
                    schemeToUse = Dtos.Layout.BackgroundRotationScheme.STARTUP;
                    break;
                case HourlySchemeName:
                    schemeToUse = Dtos.Layout.BackgroundRotationScheme.HOURLY;
                    break;
            }

            return new LayoutSettingsDto()
            {
                Id = this.Id,
                BackgroundRotationScheme = schemeToUse
            };
        }

        private const string ManualSchemeName = "Manual";
        private const string StartupSchemeName = "Startup";
        private const string HourlySchemeName = "Hourly";
    }
}
