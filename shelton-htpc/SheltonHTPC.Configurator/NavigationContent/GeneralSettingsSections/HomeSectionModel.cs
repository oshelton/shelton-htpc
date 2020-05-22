using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent.GeneralSettingsSections
{
    public sealed class HomeSectionModel : NavigationSectionModelBase<GeneralSettingsContentModel>
    {
        public HomeSectionModel(GeneralSettingsContentModel parent) : base(parent) { }

        public override string Title { get; } = "Home";

        public override bool Scrollable => false;
    }
}
