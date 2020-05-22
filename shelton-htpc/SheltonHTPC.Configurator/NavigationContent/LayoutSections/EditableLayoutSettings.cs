using SheltonHTPC.Data.Entities;
using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent.LayoutSections
{
    /// <summary>
    /// Editable Layout Settings.
    /// </summary>
    public sealed class EditableLayoutSettings : Model
    {
        /// <summary>
        /// Collection of background rotation schemes.
        /// </summary>
        public static readonly IEnumerable<string> BackgroundSchemas = new[]
        {
            ManualSchemeName,
            StartupSchemeName,
            HourlySchemeName
        };

        /// <summary>
        /// Private constructor for duplication purposes.
        /// </summary>
        private EditableLayoutSettings() { }

        /// <summary>
        /// Typically used constructor, just pass it a Dto object.
        /// </summary>
        public EditableLayoutSettings(LayoutSettingsDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            Id = dto.Id;
            switch (dto.BackgroundRotationScheme)
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

        private string _BackgroundRotationScheme = null;
        /// <summary>
        /// Currently used background rotation scheme.
        /// </summary>
        public string BackgroundRotationScheme
        {
            get => CheckIsOnMainThread(_BackgroundRotationScheme);
            set => SetPropertyBackingValue(value, ref _BackgroundRotationScheme);
        }

        /// <summary>
        /// Create a real copy of this EditableLayoutSettings object.
        /// </summary>
        public EditableLayoutSettings Duplicate()
        {
            return new EditableLayoutSettings()
            {
                Id = this.Id,
                _BackgroundRotationScheme = this.BackgroundRotationScheme
            };
        }

        /// <summary>
        /// Merge changes from another EditableLayoutSettings object.
        /// </summary>
        /// <param name="other"></param>
        public void MergeChangesFromOther(EditableLayoutSettings other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            _BackgroundRotationScheme = other._BackgroundRotationScheme;

            RaisePropertyChanged(null);
        }

        /// <summary>
        /// Construct a dto object from this view model.
        /// </summary>
        public LayoutSettingsDto CreateDto()
        {
            Dtos.Layout.BackgroundRotationScheme schemeToUse = Dtos.Layout.BackgroundRotationScheme.NONE;
            switch (_BackgroundRotationScheme)
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
