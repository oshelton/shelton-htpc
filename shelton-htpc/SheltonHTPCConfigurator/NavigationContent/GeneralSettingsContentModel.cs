using SheltonHTPC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFAspects.Core;

namespace SheltonHTPC.NavigationContent
{
    public class GeneralSettingsContentModel : NavigationContentModelBase
    {
        public override bool CanNavigateAway() => true;

        public override Task Initialize(GeneralSettings generalSettings)
        {
            _PersistedGeneralSettings = generalSettings;

            return Task.CompletedTask;
        }

        public override Task OnNavigatedTo()
        {
            BeingEditedSettingsModel = _PersistedGeneralSettings.Duplicate();

            SettingsTracker = new DirtyTracker(BeingEditedSettingsModel);
            SettingsTabTracker = SettingsTracker.CreateDirtyTrackingGroup("SettingsTab",
                nameof(BeingEditedSettingsModel.DataPath), nameof(BeingEditedSettingsModel.RunOnStartup), nameof(BeingEditedSettingsModel.IdleWaitMinutes));
            FeaturesTabTracker = SettingsTracker.CreateDirtyTrackingGroup("FeaturesTab",
                nameof(BeingEditedSettingsModel.EnableMovies), nameof(BeingEditedSettingsModel.EnableSeries), nameof(BeingEditedSettingsModel.EnableMusic), nameof(BeingEditedSettingsModel.EnablePhotos),
                nameof(BeingEditedSettingsModel.EnableGames), nameof(BeingEditedSettingsModel.EnableWebAccess), nameof(BeingEditedSettingsModel.EnableApplications), nameof(BeingEditedSettingsModel.EnableWebSites),
                nameof(BeingEditedSettingsModel.EnableWidgets));

            return Task.CompletedTask;
        }

        public override Task OnNavigatedAwayFrom()
        {
            SettingsTabTracker = null;
            FeaturesTabTracker = null;
            SettingsTracker.Dispose();
            BeingEditedSettingsModel = null;

            return Task.CompletedTask;
        }

        public override void OnSaved(object sender, RoutedEventArgs args)
        {
            _PersistedGeneralSettings.MergeChangesFromOther(BeingEditedSettingsModel);

            var justEdited = BeingEditedSettingsModel.Duplicate();
            Task.Run(() => justEdited.Serialize());

            SettingsTracker.SetInitialState();
        }

        public override void OnReset(object sender, RoutedEventArgs args)
        {
            SettingsTracker.ResetToInitialState();
        }

        public override ContentKind Kind => ContentKind.GeneralSettings;

        private GeneralSettings _BeingEditedSettingsModel;
        /// <summary>
        /// The GeneralSettings model being edited currently.
        /// </summary>
        public GeneralSettings BeingEditedSettingsModel
        {
            get => _BeingEditedSettingsModel;
            set => SetPropertyBackingValue(value, ref _BeingEditedSettingsModel);
        }

        private DirtyTracker _SettingsTracker;
        /// <summary>
        /// Tracker for changes to the General Settings object being edited.
        /// </summary>
        public DirtyTracker SettingsTracker
        {
            get => _SettingsTracker;
            set => SetPropertyBackingValue(value, ref _SettingsTracker);
        }

        private DirtyTrackingGroup _SettingsTabTracker = null;
        /// <summary>
        /// Get whether or not there have been any changes made to the settings tab.
        /// </summary>
        public DirtyTrackingGroup SettingsTabTracker
        {
            get => _SettingsTabTracker;
            set => SetPropertyBackingValue(value, ref _SettingsTabTracker);
        }

        private DirtyTrackingGroup _FeaturesTabTracker = null;
        /// <summary>
        /// Get whether or not there have been any changes made to the features tab.
        /// </summary>
        public DirtyTrackingGroup FeaturesTabTracker
        {
            get => _FeaturesTabTracker;
            set => SetPropertyBackingValue(value, ref _FeaturesTabTracker);
        }

        private GeneralSettings _PersistedGeneralSettings = null;
    }
}
