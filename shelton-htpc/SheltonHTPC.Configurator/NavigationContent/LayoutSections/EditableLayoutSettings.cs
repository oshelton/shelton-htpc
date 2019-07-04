using SheltonHTPC.Data.Entities;
using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheltonHTPC.NavigationContent.LayoutSections
{
    /// <summary>
    /// Editable Layou Settings.
    /// </summary>
    public sealed class EditableLayoutSettings : LayoutSettings
    {
        /// <summary>
        /// Private constructor for duplication purposes.
        /// </summary>
        private EditableLayoutSettings() { }

        /// <summary>
        /// Typically used constructor, just pass it a Dto object.
        /// </summary>
        public EditableLayoutSettings(LayoutSettingsDto dto)
            : base(dto)
        {
        }

        /// <summary>
        /// Create a real copy of this EditableLayoutSettings object.
        /// </summary>
        public EditableLayoutSettings Duplicate()
        {
            return new EditableLayoutSettings()
            {
                _BackgroundRotationScheme = this.BackgroundRotationScheme
            };
        }

        /// <summary>
        /// Merge changes from another EditableLayoutSettings object.
        /// </summary>
        /// <param name="other"></param>
        public void MergeChangesFromOther(EditableLayoutSettings other)
        {
            _BackgroundRotationScheme = other._BackgroundRotationScheme;

            RaisePropertyChanged(null);
        }
    }
}
