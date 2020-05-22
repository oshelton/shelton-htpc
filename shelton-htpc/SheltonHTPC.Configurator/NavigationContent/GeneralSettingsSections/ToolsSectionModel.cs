using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SheltonHTPC.Data.Entities;
using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFAspects.Core;
using WPFAspects.Utils;

namespace SheltonHTPC.NavigationContent.GeneralSettingsSections
{
    public sealed class ToolsSectionModel : NavigationSectionModelBase<GeneralSettingsContentModel>
    {
        public ToolsSectionModel(GeneralSettingsContentModel parent) : base(parent) { }

        public override string Title { get; } = "Tools";

        public override bool Scrollable => true;

        public async void DoPurge(object sender, RoutedEventArgs e)
        {
            if (sender is null)
                throw new ArgumentNullException(nameof(sender));

            var button = sender as Button;
            var window = Window.GetWindow(button) as MetroWindow;
            var context = (button.DataContext as ToolsSectionModel).Parent.BeingEditedSettingsModel;

            var result = await window.ShowMessageAsync("Confirm Data Reset", "If you continue all Shelton HTPC data will be removed and you will not be able to get it back (except by restoring a previously made backup).  The application will also be restarted.\n\nAre you sure you want to continue?", MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);

            if (result == MessageDialogResult.Affirmative)
                await WorkManager.StartApplicationBlockingWork(() => Utils.DataTools.PurgeData(context), "Purging data...").ConfigureAwait(true);
        }
    }
}
